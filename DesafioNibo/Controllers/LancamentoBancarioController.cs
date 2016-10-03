using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using DesafioNibo.Dominio;
using DesafioNibo.Models;
using DesafioNibo.Services;

namespace DesafioNibo.Controllers
{
    public class LancamentoBancarioController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LancamentoBancarios
        public ActionResult Index()
        {
            return View(db.TransacaoBancarias.OrderBy(p => p.DataDePostagem).ToList());
        }

        // GET: LancamentoBancarios/Create
        public ActionResult Create()
        {
            return View(new LancamentoBacarioViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LancamentoBacarioViewModel  lancamentoBacarioViewModel)
        {
            if (!ModelState.IsValid) return View(lancamentoBacarioViewModel);
            try
            {
                var lancamentoBancarioAppService = new LancamentoBancarioAppService();
                var lancamentos = lancamentoBancarioAppService.EnviaLancamentos(lancamentoBacarioViewModel);

                db.TransacaoBancarias.AddRange(lancamentos);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }

            return RedirectToAction("Index");
        }

        // GET: LancamentoBancarios/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var lancamentoBancario = db.TransacaoBancarias.Find(id);
            if (lancamentoBancario == null)
            {
                return HttpNotFound();
            }
            return View(lancamentoBancario);
        }

        // POST: LancamentoBancarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var lancamentoBancario = db.TransacaoBancarias.Find(id);
            db.TransacaoBancarias.Remove(lancamentoBancario);
            db.SaveChanges();
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
