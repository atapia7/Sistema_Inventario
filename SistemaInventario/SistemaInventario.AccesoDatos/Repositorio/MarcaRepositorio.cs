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
    public  class MarcaRepositorio : Repositorio<Marca>, IMarcaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public MarcaRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Actualizar(Marca marca)
        {
          var marcaDB=_db.Bodegas.FirstOrDefault(b=>b.Id== marca.Id);
            if (marcaDB != null)
            {
                marcaDB.Nombre=marca.Nombre;
                marcaDB.Descripcion=marca.Descripcion;
                marcaDB.Estado=marca.Estado;
                _db.SaveChanges();
            }
        }
    }
}
