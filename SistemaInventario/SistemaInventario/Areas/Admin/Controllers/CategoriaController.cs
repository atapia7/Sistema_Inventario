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
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria categoria = new Categoria();
            if (id == null)
            {
                //Add new categoria
                categoria.Estado = true;
                return View(categoria);
            }
            //update categoria
            categoria = await _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);

        }
       
 #region APIS
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new{data = todos });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
            public async Task<IActionResult> Upsert(Categoria categoria)
            {
                if (ModelState.IsValid)
                {
                    if (categoria.Id == 0)
                    {
                        await _unidadTrabajo.Categoria.Agregar(categoria);
                        TempData[DS.Exitosa] = "Categoria creada exitosamente";
                    }else{
                        _unidadTrabajo.Categoria.Actualizar(categoria);
                        TempData[DS.Exitosa] = "Categoria Actualizada exitosamente";
                    }
                    await _unidadTrabajo.Guardar();
                    return RedirectToAction(nameof(Index));
                }
                TempData[DS.Error] = "Error al grabar Categoria";
                return View(categoria);
            }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegaDb = await _unidadTrabajo.Categoria.Obtener(id);
            if (bodegaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Categoria" });
            }
            _unidadTrabajo.Categoria.Remover(bodegaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valid = false;
            var lista = await _unidadTrabajo.Categoria.ObtenerTodos();
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
