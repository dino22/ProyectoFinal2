using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Controllers.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UsuarioController : ControllerBase
    {
        [HttpGet(Name = "Traer Usuarios")]
        public List<Usuario> TraerUsuarios([FromHeader] string NombreUsuario)
        {
            return UsuarioHandler.TraerUsuarios(NombreUsuario);
        }
                
        [HttpPost(Name = "Post Usuario")]
        public string AgregarUsuario([FromBody] PostUsuario usu)
        {
            return UsuarioHandler.AgregarUsuario(new Usuario
            {
                Nombre = usu.Nombre,
                Apellido = usu.Apellido,
                NombreUsuario = usu.NombreUsuario,
                Contraseña = usu.Contraseña,
                Mail = usu.Mail
            });
        }

        [HttpPut(Name = "Put Usuario")]
        public string ModificarUsuario([FromBody] PutUsuario usu)
        {
            return UsuarioHandler.ModificarUsuario(new Usuario
            {
                Id = usu.Id,
                Nombre = usu.Nombre,
                Apellido = usu.Apellido,
                NombreUsuario = usu.NombreUsuario,
                Contraseña = usu.Contraseña,
                Mail = usu.Mail
            });
        }

        [HttpDelete(Name = "Delete Usuario")]
        public string QuitarUsuario([FromHeader] int usu)
        {
            return UsuarioHandler.QuitarUsuario(usu);
        }
    }
}
