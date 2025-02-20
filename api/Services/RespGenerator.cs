using Microsoft.AspNetCore.Mvc;

namespace App.Services;

public class RespGenerator
{
    public object generate(object data, string errorMessage) {
        if (string.IsNullOrEmpty(errorMessage)) {
            return new {
                status = 200,
                data = data, // data n'est pas null en cas de succès
                error = (object)null
            };
        }
        else {
            return new
            {
                status = 400,
                data = (object)null, // data est null en cas d'erreur
                error = new
                {
                    message = errorMessage
                }
            };
        }
    }
}