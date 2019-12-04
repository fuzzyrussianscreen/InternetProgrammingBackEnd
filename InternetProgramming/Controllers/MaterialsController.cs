using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using InternetProgramming.Models;
using System.Web.Http.Cors;
using System.Data.SqlClient;
using System.Dynamic;

namespace InternetProgramming.Controllers
{
    [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
    public class MaterialsController : ApiController
    {
        private WebAPIDBEntities db = new WebAPIDBEntities();


        // GET: api/Materials
        public IQueryable<Material> GetMaterials()
        {
            return db.Materials;
        }

        // GET: api/Materials/5
        [ResponseType(typeof(Material))]
        public IHttpActionResult GetMaterial(int id)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        // GET: api/SearchMaterials
        [HttpPost]
        [Route("api/SearchMaterials")]
        [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
        public IEnumerable<Material> SearchMaterials(string text)
        {
            string sqlExpression = "Search";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add("@text", SqlDbType.VarChar);
                command.Parameters["@text"].Value = text;
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = CommandType.StoredProcedure;
                var reader = command.ExecuteReader();
                List<Material> materials = new List<Material>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        Decimal price = reader.GetDecimal(2);
                        string description = reader.GetString(3);
                        materials.Add(new Material()
                        {
                            Id = id,
                            Name = name,
                            Price = price,
                            Description = description
                        });
                    }
                }
                reader.Close();
                return materials;
            }

        }

        // PUT: api/Materials/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMaterial(int id, Material material)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != material.Id)
            {
                return BadRequest();
            }

            db.Entry(material).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Materials
        [ResponseType(typeof(Material))]
        public IHttpActionResult PostMaterial(Material material)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            db.Materials.Add(material);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MaterialExists(material.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = material.Id }, material);
        }

        // DELETE: api/Materials/5
        [ResponseType(typeof(Material))]
        public IHttpActionResult DeleteMaterial(int id)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return NotFound();
            }

            db.Materials.Remove(material);
            db.SaveChanges();

            return Ok(material);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MaterialExists(int id)
        {
            return db.Materials.Count(e => e.Id == id) > 0;
        }
    }
}