using App.Context;
using App.Models;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace App.Services;

public class UserService
{
    private readonly MyDbContext _context;

    public UserService(MyDbContext ctx) {
        _context = ctx;
    }


    public List<User> get_all_users() {
        return _context.Users.ToList();
    }


    public User get_user_by_email(string email) {
        User simpleUser =  _context.Users.Where(u => u.email == email)
         .FirstOrDefault();
        if(simpleUser != null) return simpleUser;
        throw new Exception("Your email '"+email+"' does not exist !");
    }


    public void reset_attempt_for_user(int user_id) {
        User user = _context.Users.FirstOrDefault(u => u.id == user_id);

        if(user != null) {
            int maxCountAttempt = _context.NumberAttempts.Select(c => c.count)
                .FirstOrDefault();

            var userAttempt = _context.Attempts
                .FirstOrDefault(a=>a.user_id==user.id);
            if(userAttempt != null) {
                userAttempt.attempt = maxCountAttempt;
                _context.SaveChanges();
            }
        }
        else throw new Exception("User with id="+user_id+" does not exist");
    }


    public string generate_pin() {
        var random = new Random();
        var result = string.Empty;

        for (int i=0 ; i<6 ; i++) {
            result += random.Next(0, 10).ToString();
        }

        return result;
    }


    public void send_email(string recipient, string subject, string text) {
        var message = new MimeMessage();
    // Email de l'envoyeur
        message.From.Add(new MailboxAddress("Identity-Provider", "fra15.manantsoa@gmail.com"));
    // Email du destinataire
        message.To.Add(new MailboxAddress(recipient, recipient));
    // Sujet de l'email
        message.Subject = subject;
    // Format du texte
        message.Body = new TextPart(TextFormat.Plain)
        {
            Text = text
        };

    // Créez une instance du client SMTP
        using (var client = new SmtpClient())
        {
        // Se connecter à Gmail avec les paramètres SMTP
            client.Connect("smtp.gmail.com", 587, false); // SMTP de Gmail
            client.Authenticate("fra15.manantsoa@gmail.com", "fyxz nvgi rczf wpvf");
        // Envoyer l'email
            client.Send(message);
        // Se déconnecter du serveur SMTP après l'envoi
            client.Disconnect(true);
        }
    }


    public void send_reset_attempt_email(string email) {
         var message = new MimeMessage();
    // Email de l'envoyeur
        message.From.Add(new MailboxAddress("Identity-Provider", "fra15.manantsoa@gmail.com"));
    // Email du destinataire
        message.To.Add(new MailboxAddress(email, email));
    // Sujet de l'email
        message.Subject = "New attempts for sign in";
        User user = this.get_user_by_email(email);
    // Format du texte
        message.Body = new TextPart(TextFormat.Plain)
        {
            Text = "Here is your link to reset your attempt: http://localhost:5000/api/reset/"+user.id
        };

    // Créez une instance du client SMTP
        using (var client = new SmtpClient())
        {
        // Se connecter à Gmail avec les paramètres SMTP
            client.Connect("smtp.gmail.com", 587, false); // SMTP de Gmail
            client.Authenticate("fra15.manantsoa@gmail.com", "fyxz nvgi rczf wpvf");
        // Envoyer l'email
            client.Send(message);
        // Se déconnecter du serveur SMTP après l'envoi
            client.Disconnect(true);
        }
    }


    public void check_attempt_if_zero(Attempt userAttempt, User user) {
        if(userAttempt.attempt==0) {
            string link = "http://localhost:5000/api/reset/"+user.id+"";

            this.send_email(user.email, "Reset of attempt number",
                "Go to this link to unlock your account "+link+"");
            throw new Exception("Your account is blocked, check your email to unlock it !");
        }
    }


    public void log_first_step(LoginRequest1 logReq) {
        var user = this.get_user_by_email(logReq.email);
        if(user == null) {
            throw new Exception("Email '"+logReq.email+"' does not exist!");
        }
    // Le tentative pour l'utilisateur
        int maxCountAttempt = _context.NumberAttempts.Select(c => c.count).FirstOrDefault();
        var userAttempt = _context.Attempts.Where(a => a.user_id==user.id)
            .FirstOrDefault();
        if(userAttempt==null) {
            Attempt attempt = new Attempt() {
                attempt = maxCountAttempt,
                user_id = user.id
            };
            _context.Attempts.Add(attempt); 
            _context.SaveChanges();
            userAttempt = _context.Attempts.Where(a => a.user_id==user.id)
                .FirstOrDefault();
        }
        if(userAttempt.attempt==0) throw new Exception("Your account is blocked, check your email to unlock it !");

        bool isLogged = BCrypt.Net.BCrypt.Verify(logReq.password, user.password);
    // Email et mot de passe correcte
        if(isLogged) {
            string email = logReq.email;
            string pin_code = this.generate_pin();
            DateTime expiration = _context.PinExpirations
                .Where(st => st.id==1)
                .Select(st => DateTime.UtcNow.Add(st.time)).FirstOrDefault();

        // Creation d'authentification ou modification
            var auth = _context.Auths.Where(a => a.user_id==user.id)
                .FirstOrDefault();
            if(auth != null) { // UPDATE
                auth.pin_code = pin_code;
                auth.expiration = expiration;
            }
            else { // INSERT
                Authentification newAuth = new Authentification() {
                    pin_code = pin_code,
                    expiration = expiration,
                    user_id = user.id
                };
                _context.Auths.Add(newAuth);
            }
            _context.SaveChanges();

        // Envoi du code PIN dans l'Email de l'utilisateur
            string link = "http://localhost:5000/api/login/steps/2/"+user.id;
            this.send_email(email, "Authentification step 2", 
                "Here is your PIN code to continue: "+pin_code+"\n"+
                "And this is your next link: "+link);

        // Réinitialisation de tentative si connécté en 1e étape
            // userAttempt.attempt = maxCountAttempt;
            // _context.SaveChanges();
        }
        else { // Le nombre de tentative diminue
            userAttempt.attempt -= 1;
            _context.SaveChanges();
            this.check_attempt_if_zero(userAttempt, user);
            throw new Exception("Your email or your password doesn't exist !");
        }
    }


    public string generate_token(int user_id) {
        var claims = new[]
        {
            new Claim("user_id", user_id.ToString()),
            // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Identifiant unique
            // new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64) // Date d'émission
        };

        // Clé de signature
        string keyString = "u?yy9!tuy8!0dk,ihrg%zrg;ryibtfgun<mn jdk";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Construire le token avec une expiration (facultatif)
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(5)
        );

        // Générer le token sous forme de chaîne
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public void log_second_step(int user_id, LoginRequest2 pin) {
        string pin_code = pin.pin;
        int maxCountAttempt = _context.NumberAttempts.Select(c => c.count)
            .FirstOrDefault();
        User user = _context.Users.Find(user_id);
        if(user == null) {
            throw new Exception("User with id="+user_id+" does not exist !");
        }
    // Authentification
        var auth = _context.Auths.Where(a => 
            a.pin_code==pin_code &&
            DateTime.UtcNow<=a.expiration && 
            a.user_id==user.id).FirstOrDefault();
    // Tentative pour l'utilisateur
        var userAttempt = _context.Attempts.Where(a => a.user_id==user.id).FirstOrDefault();
        if(userAttempt.attempt==0) {
            throw new Exception("Your account is blocked, check your email to unlock it !");
        }
        
    // Tester si authentifié
        if(auth != null) {
            userAttempt.attempt = maxCountAttempt;

        // Vérifier si token éxiste
            var userToken = _context.Sessions.Where(s => s.user_id==user.id)
                .FirstOrDefault();
            if(userToken != null) { // Deja inscrit
                userToken.token = this.generate_token(user.id);
                userToken.expiration = _context.SessionExpirations
                    .Where(e => e.id==1)
                    .Select(e => DateTime.UtcNow.Add(e.time)).FirstOrDefault();
            }
            else { // Pas encore de Token
                var userSession = new Session() {
                    token = this.generate_token(user.id),
                    expiration = _context.SessionExpirations
                        .Where(e => e.id==1)
                        .Select(e => DateTime.UtcNow.Add(e.time)).FirstOrDefault(),
                    user_id = user.id
                };
                _context.Sessions.Add(userSession);
            }
            _context.SaveChanges();

        // Envoyer en email le lien avec le token
            string link1 = "http://localhost:5000/api/users/"+user.id+"/profile?token="
                +userToken.token;
            string link2 = "http://localhost:5000/api/users/"+user.id+"?token="
                +userToken.token;
            // string link3 = "http://localhost:5000/api/users/"+user.id+"?token="
            //     +userToken.token;
            this.send_email(user.email, "Link into your profile", 
             "Your profile: "+link1+"\n"+
             "To update: "+link2);
        }
        else {
            userAttempt.attempt -= 1;
            _context.SaveChanges();
            this.check_attempt_if_zero(userAttempt, user);

            throw new Exception("This PIN code is incorrect or expired !");
        }
    }


// INSCRIPTION
    public void sign_up(User user) {
    // Insertion de l'utilisateur
        User new_user = new User() {
            name = user.name, 
            email = user.email,
            password = BCrypt.Net.BCrypt.HashPassword(user.password)
        };
        _context.Users.Add(new_user);
        _context.SaveChanges();

    // Insertion du token
        string userToken = this.generate_token(new_user.id);
        Session session = new Session() {
            user_id = new_user.id,
            token = userToken,
            expiration =  _context.SessionExpirations
                .Where(e => e.id==1)
                .Select(e => DateTime.UtcNow.Add(e.time)).FirstOrDefault()
        };
        _context.Sessions.Add(session);

    // Sauvegarder les insertions
        _context.SaveChanges();

    // Envoi d'email
        string link1 = "http://localhost:5000/api/users/"+new_user.id+"/profile?token="
            +session.token;
        string link2 = "http://localhost:5000/api/users/"+new_user.id+"?token="
            +session.token;
        // string link3 = "http://localhost:5000/api/users/"+user.id+"?token="
        //     +session.token;
        this.send_email(new_user.email, "Link into your profile", 
            "Your profile: "+link1+"\n"+
            "To update: "+link2);
    }


    public void check_validation_for_token(int user_id, string token) {
        var userSession = _context.Sessions.Where(s => 
            s.token==token && 
            s.user_id==user_id &&
            DateTime.UtcNow<=s.expiration).FirstOrDefault();
        
        if(userSession == null) {
            throw new Exception("You should reconnect because your session is expired !");
        }
    }


    public object get_profile(int user_id, string token) {
        this.check_validation_for_token(user_id, token);
        
    // Token encore valide
        var user_info = (from usr in _context.Users
            where usr.id == user_id
            select new
            {
                id = usr.id,
                name = usr.name,
                email = usr.email,
                password = usr.password,
                attempt = (from att in _context.Attempts where att.user_id == usr.id select att.attempt).FirstOrDefault(),
                pin_code = (from aut in _context.Auths where aut.user_id == usr.id select aut.pin_code).FirstOrDefault(),
                pin_expiration = (from aut in _context.Auths where aut.user_id == usr.id select aut.expiration).FirstOrDefault(),
                token = (from exp in _context.Sessions where exp.user_id == usr.id select exp.token).FirstOrDefault(),
                token_expiration = (from exp in _context.Sessions where exp.user_id == usr.id select exp.expiration).FirstOrDefault()
            }).FirstOrDefault();

        if(user_info==null) {
            throw new Exception("Profile not found or invalid token.");
        }
        return user_info;
    }


    public void update(User user, User new_user) {
        user.name = new_user.name;
        user.email = new_user.email;
        user.password = BCrypt.Net.BCrypt.HashPassword(new_user.password);

        _context.SaveChanges();
    }


    public void update_info(int user_id, string token, User new_user) {
        this.check_validation_for_token(user_id, token);

        User user = _context.Users.Find(user_id);
        if(user==null) throw new Exception("User with id="+user_id+" not found !");
        this.update(user, new_user);
    }


    // public void delete(User user) {
    //     _context.Users.Remove(user);

    //     _context.SaveChanges();
    // }


    // public void delete_user(int user_id, string token) {
    //     this.check_validation_for_token(user_id, token);

    //     User user = _context.Users.Find(user_id);
    //     if(user==null) throw new Exception("User with id="+user_id+" not found !");
    //     this.delete(user);
    // }
}
