using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using DesafioNibo.Dominio;
using DesafioNibo.Dominio.Exceptions;
using DesafioNibo.Models;
using DesafioNibo.Services;

namespace DesafioNibo.Controllers
{
    public class TransacaoBancariasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TransacaoBancarias
        public ActionResult Index()
        {
            return View(db.TransacaoBancarias.ToList());
        }
        // GET: TransacaoBancarias/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LancamentoBancario lancamentoBancario = db.TransacaoBancarias.Find(id);
            if (lancamentoBancario == null)
            {
                return HttpNotFound();
            }
            return View(lancamentoBancario);
        }
        // GET: TransacaoBancarias/Create
        public ActionResult Create()
        {
            return View(new TransacaoBancariaViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransacaoBancariaViewModel transacaoBancariaViewModel)
        {
            if (!ModelState.IsValid) return View(transacaoBancariaViewModel);
            try
            {
                var lancamentoBancarioAppService = new LancamentoBancarioAppService();

                string[] lines;
                using (var input = transacaoBancariaViewModel.Arquivo.InputStream)
                {
                    using (var sr = new StreamReader(input)) lines = sr.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
                }

                var lancamentosBancarios = lancamentoBancarioAppService.PopulaLancamentosBancarios(lines);
                db.TransacaoBancarias.AddRange(lancamentosBancarios);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }            

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
