\# Autorizacion.Middleware



Middleware de ASP.NET Core 8 que consulta los perfiles (roles) del usuario

autenticado en la base de datos de seguridad y los agrega como ‘Claim’

al ‘HttpContext.User’.



\## Paso 1 — Configurar el feed de GitHub Packages (una sola vez por máquina)



Crea un PAT en tu cuenta GitHub con scope ‘read:packages’, luego ejecuta:



```powershell

dotnet nuget add source https://nuget.pkg.github.com/SC-701/index.json `

&#x20; --name github `

&#x20; --username TU\_USUARIO\_GITHUB `

&#x20; --password TU\_PERSONAL\_ACCESS\_TOKEN `

&#x20; --store-password-in-clear-text

