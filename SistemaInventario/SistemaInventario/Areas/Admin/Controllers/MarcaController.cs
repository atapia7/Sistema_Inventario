using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
   //[Authorize(Roles =DS.Role_Admin)]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            Marca marca = new Marca();
            if (id == null)
            {
                //Add new marca
                marca.Estado = true;
                return View(marca);
            }
            //update marca
            marca = await _unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);

        }
        #region APIS

        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Marca.ObtenerTodos();
            return Json(new{data = todos });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
            public async Task<IActionResult> Upsert(Marca marca)
            {
                if (ModelState.IsValid)
                {
                    if (marca.Id == 0)
                    {
                        await _unidadTrabajo.Marca.Agregar(marca);
                        TempData[DS.Exitosa] = "Marca creada exitosamente";
                    }else{
                        _unidadTrabajo.Marca.Actualizar(marca);
                        TempData[DS.Exitosa] = "Marca Actualizada exitosamente";
                    }
                    await _unidadTrabajo.Guardar();
                    return RedirectToAction(nameof(Index));
                }
                TempData[DS.Error] = "Error al grabar Marca";
                return View(marca);
            }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegaDb = await _unidadTrabajo.Marca.Obtener(id);
            if (bodegaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Marca" });
            }
            _unidadTrabajo.Marca.Remover(bodegaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Marca borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valid = false;
            var lista = await _unidadTrabajo.Marca.ObtenerTodos();
            if (id == 0)
            {
                valid = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                valid = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id !=id);
            }
            if (valid)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });

        }
        #endregion
    }
}
