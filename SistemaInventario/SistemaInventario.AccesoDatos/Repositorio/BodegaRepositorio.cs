using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public  class BodegaRepositorio : Repositorio<Bodega>, IBodegaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public BodegaRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Actualizar(Bodega bodega)
        {
          var bodegaDB=_db.Bodegas.FirstOrDefault(b=>b.Id== bodega.Id);
            if (bodegaDB != null)
            {
                bodegaDB.Nombre=bodega.Nombre;
                bodegaDB.Descripcion=bodega.Descripcion;
                bodegaDB.Estado=bodega.Estado;
                _db.SaveChanges();
            }
        }
    }
}
