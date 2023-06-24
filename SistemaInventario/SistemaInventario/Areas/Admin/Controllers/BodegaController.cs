﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
   //[Authorize(Roles =DS.Role_Admin)]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            Bodega bodega = new Bodega();
            if (id is null)
            {
                //Add new bodega
                bodega.Estado = true;
                return View(bodega);
            }
            //update bodega
            bodega = await _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if (bodega is null)
            {
                return NotFound();
            }
            return View(bodega);

        }
        #region APIS

        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new{data = todos });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
            public async Task<IActionResult> Upsert(Bodega bodega)
            {
                if (ModelState.IsValid)
                {
                    if (bodega.Id == 0)
                    {
                        await _unidadTrabajo.Bodega.Agregar(bodega);
                        TempData[DS.Exitosa] = "Bodega creada exitosamente";
                    }else{
                        _unidadTrabajo.Bodega.Actualizar(bodega);
                        TempData[DS.Exitosa] = "Bodega Actualizada exitosamente";
                    }
                    await _unidadTrabajo.Guardar();
                    return RedirectToAction(nameof(Index));
                }
                TempData[DS.Error] = "Error al grabar Bodega";
                return View(bodega);
            }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegaDb = await _unidadTrabajo.Bodega.Obtener(id);
            if (bodegaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Bodega" });
            }
            _unidadTrabajo.Bodega.Remover(bodegaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valid = false;
            var lista = await _unidadTrabajo.Bodega.ObtenerTodos();
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
