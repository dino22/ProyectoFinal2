using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Controllers.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UsuarioController : ControllerBase
    {
        //Método API para traer Usuario por nombreUsuario si existe
        [HttpGet(Name = "Traer Usuarios")]
        public List<Usuario> TraerUsuarios([FromHeader] string nombreUsuario)
        {
            return UsuarioHandler.TraerUsuarios(nombreUsuario);
        }
        //Método API para agregar un Usuario
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
        //Método API para modificar un Usuario
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
        //Método API para eliminar un Usuario
        [HttpDelete(Name = "Delete Usuario")]
        public string EliminarUsuario([FromHeader] int usu)
        {
            return UsuarioHandler.EliminarUsuario(usu);
        }
    }
}
