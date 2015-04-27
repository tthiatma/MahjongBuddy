using MahjongBuddy.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MahjongBuddy.Service.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/
        enum TileType
        {
            OneToNine,
            Dragon,
            Wind,
            Flower
        }


        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Game/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Game/Create

        public ActionResult Create()
        {
            CompleteTile completeTiles = new CompleteTile();
            foreach (string ttype in Enum.GetValues(typeof(TileType)))
            {
                Tile tempTile = new Tile();
                switch (ttype)
                {
                    case "OneToNine" :
                        tempTile.TileType = new OneToNineTile();   
                        break;

                    case "Dragon" :
                        tempTile.TileType = new DragonTile();
                        break;

                    case "Wind":
                        tempTile.TileType = new DragonTile();
                        break;

                    case "Flower":
                        tempTile.TileType = new FlowerTile();
                        break;                
                }

                //make the tile type
                for (var i = 0; i < tempTile.TileType.TileTypeCount; i++)
                {
                    //make 4 sets for each of it
                    for (var x = 0; x < tempTile.BaseTileCount; x++)
                    {
                        completeTiles.Tiles.Add(new Tile() {TileType = tempTile.TileType, TileImagePath = "" });
                    }
                }

            }
            

            return View();
        }

        //
        // POST: /Game/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Game/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Game/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Game/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Game/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
