using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos.Seguridad;
using Abstracciones.Seguridad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Web.Pages.Cuenta
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginRequest loginInfo { get; set; } = default!;

        [BindProperty]
        public Token token { get; set; } = default!;

        private readonly IConfiguracion _configuracion;

        public LoginModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // 🔹 Hash password
            var hash = Autenticacion.GenerarHash(loginInfo.Password);
            loginInfo.PasswordHash = Autenticacion.ObtenerHash(hash);

            // 🔹 Username desde correo
            loginInfo.NombreUsuario = loginInfo.CorreoElectronico.Split("@")[0];

            // 🔹 Endpoint login
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPointsSeguridad", "Login");

            var client = new HttpClient();

            var respuesta = await client.PostAsJsonAsync<LoginBase>(endpoint,
                new LoginBase
                {
                    NombreUsuario = loginInfo.NombreUsuario,
                    CorreoElectronico = loginInfo.CorreoElectronico,
                    PasswordHash = loginInfo.PasswordHash
                });

            respuesta.EnsureSuccessStatusCode();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var contenido = await respuesta.Content.ReadAsStringAsync();

            token = JsonSerializer.Deserialize<Token>(contenido, opciones);

            if (token != null && token.ValidacionExitosa && !string.IsNullOrEmpty(token.AccessToken))
            {
                // 🔹 Leer JWT
                JwtSecurityToken? jwtToken = Autenticacion.leerToken(token.AccessToken);

                // 🔹 Generar claims (IMPORTANTE)
                var claims = Autenticacion.GenerarClaims(jwtToken, token.AccessToken);

                await establecerAutenticacion(claims);

                var urlredirigir = $"{HttpContext.Request.Query["ReturnUrl"]}";

                if (string.IsNullOrEmpty(urlredirigir))
                    return Redirect("/");

                return Redirect(urlredirigir);
            }

            ModelState.AddModelError(string.Empty, "Credenciales inválidas");
            return Page();
        }

        private async Task establecerAutenticacion(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        }
    }
}