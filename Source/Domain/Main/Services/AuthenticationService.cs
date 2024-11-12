using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;

namespace TaskListMaui.Source.Domain.Main.Services
{
    public static class AuthenticationService
    {

        public static async Task<Response> SaveToken(string token)
        {            
            try
            {
                await SecureStorage.Default.SetAsync("token", token);

                return new Response($"Token armazenado com sucesso.\n{token}", 200);
            }
            catch (Exception ex)
            {
                return new Response($"Erro ao armazenar o token. {ex.Message} - {ex.InnerException}", 400);
            }
        }

        public static async Task<string> GetToken()
        {           
            string token = await SecureStorage.Default.GetAsync("token") ?? string.Empty;         
            return token;           
        }

        public static Response RemoveToken()
        {
            try
            {
                SecureStorage.Default.Remove("token");
                return new Response("Token removido com sucesso.", 200);
            }
            catch (Exception ex)
            {
                return new Response($"Erro ao remover o token. {ex.Message} - {ex.InnerException}",400);
            }
        }

        public static async Task<string> GetUserId()
        {
            var handlerToken = new JwtSecurityTokenHandler();

            var token = await GetToken();

            var jwtToken = handlerToken.ReadJwtToken(token?.ToString());

            string id = jwtToken.Claims.FirstOrDefault(c => c.Type == "subject")?.Value ?? string.Empty;

            return id;

        }

    }
}
