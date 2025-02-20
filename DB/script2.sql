-- Schema

CREATE TABLE session_timing(
   session_id SERIAL,
   expiration TIME NOT NULL,
   PRIMARY KEY(session_id)
);

CREATE TABLE compte_tentative(
   compte_tentative_id SERIAL,
   compte INTEGER NOT NULL,
   PRIMARY KEY(compte_tentative_id)
);

CREATE TABLE users(
   user_id SERIAL,
   user_name VARCHAR(70)  NOT NULL,
   user_email VARCHAR(70)  NOT NULL,
   user_password VARCHAR(255)  NOT NULL,
   user_token VARCHAR(255) ,
   PRIMARY KEY(user_id),
   UNIQUE(user_email)
);

CREATE TABLE attempts(
   attempt_id SERIAL,
   attempt INTEGER NOT NULL,
   user_id INTEGER NOT NULL,
   PRIMARY KEY(attempt_id),
   UNIQUE(user_id),
   FOREIGN KEY(user_id) REFERENCES users(user_id)
);

CREATE TABLE pin_expirations(
   expiration_id SERIAL,
   expiration TIME NOT NULL,
   PRIMARY KEY(expiration_id)
);

CREATE TABLE number_attempts(
   number_attempt_id SERIAL,
   number_attempt INTEGER NOT NULL,
   PRIMARY KEY(number_attempt_id)
);

CREATE TABLE authentifications(
   auth_id SERIAL,
   pin VARCHAR(50)  NOT NULL,
   expiration TIMESTAMP NOT NULL,
   user_id INTEGER NOT NULL,
   PRIMARY KEY(auth_id),
   UNIQUE(user_id),
   FOREIGN KEY(user_id) REFERENCES users(user_id)
);

CREATE TABLE sessions(
   session_id SERIAL,
   token VARCHAR(255)  NOT NULL,
   expiration TIMESTAMP NOT NULL,
   user_id INTEGER NOT NULL,
   PRIMARY KEY(session_id),
   UNIQUE(user_id),
   FOREIGN KEY(user_id) REFERENCES users(user_id)
);

CREATE TABLE session_expirations(
   session_expiration_id SERIAL,
   expiration TIME NOT NULL,
   PRIMARY KEY(session_expiration_id)
);