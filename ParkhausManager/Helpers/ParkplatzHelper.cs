using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkhausManager.Helpers
{
    public enum ParkplatzTyp
    {
        Frei,
        Gelegenheitsnutzer,
        Dauermieter
    }

    public class ParkplatzHelper
    {
        private ParkhausEntities db = new ParkhausEntities();

        public int GetAnzahlFreieParkplaetzeAufStockwerk(Stockwerk stockwerk)
        {
            var mieterCount = db.Dauermieter.Where(s => s.Stockwerk.Id.Equals(stockwerk.Id)).Count();
            var ticketCount = db.Ticket.Where(s => s.Stockwerk.Id.Equals(stockwerk.Id) && s.Bezahlt == false).Count();
            var anzParkplaetze = (stockwerk.AnzahlParkplaetze == null) ? 0 : stockwerk.AnzahlParkplaetze;
            return (int)(anzParkplaetze - mieterCount - ticketCount);
        }


        public List<int> GetBesetzteParkplatzNummernAufStockwerk(Stockwerk stockwerk)
        {
            var dauermieterNummern = db.Dauermieter.Where(s => s.Stockwerk.Id.Equals(stockwerk.Id) && s.Gesperrt == false).Select(s => s.ParkplatzNummer).ToList();
            var ticketNummern = db.Ticket.Where(s => s.Stockwerk.Id.Equals(stockwerk.Id) && s.Bezahlt == false).Select(s => s.ParkplatzNummer).ToList();
            var nummern = dauermieterNummern.Concat(ticketNummern).ToList();
            nummern.Sort();
            return nummern;
        }

        public Dictionary<int, ParkplatzTyp> GetParkplaetzeUndNummernAufStockwerk(Stockwerk stockwerk)
        {
            var parkplaetzeAufStockwerk = new Dictionary<int, ParkplatzTyp>();
            var dauermieterNummern = db.Dauermieter.Where(s => s.Stockwerk.Id.Equals(stockwerk.Id) && s.Gesperrt == false).Select(s => s.ParkplatzNummer).ToList();
            var ticketNummern = db.Ticket.Where(s => s.Stockwerk.Id.Equals(stockwerk.Id) && s.Bezahlt == false).Select(s => s.ParkplatzNummer).ToList();


            var index = stockwerk.Parkhaus.Stockwerk.ToList().IndexOf(stockwerk);
            var stockwerkFloor = index * 100;

            for(var i = stockwerkFloor; i < stockwerkFloor + stockwerk.AnzahlParkplaetze; i++)
            {
                if (dauermieterNummern.Contains(i))
                {
                    parkplaetzeAufStockwerk.Add(i, ParkplatzTyp.Dauermieter);
                }
                else if (ticketNummern.Contains(i))
                {
                    parkplaetzeAufStockwerk.Add(i, ParkplatzTyp.Gelegenheitsnutzer);
                } 
                else
                {
                    parkplaetzeAufStockwerk.Add(i, ParkplatzTyp.Frei);
                }
            }
            return parkplaetzeAufStockwerk;
        }

        public Dictionary<int, ParkplatzTyp> GetParkplaetzeUndNummernInParkhaus(Parkhaus parkhaus)
        {
            var parkplaetze = new Dictionary<int, ParkplatzTyp>();

            foreach(var stockwerk in parkhaus.Stockwerk)
            {
                var pps = GetParkplaetzeUndNummernAufStockwerk(stockwerk);
                if(pps != null && pps.Count > 0)
                {
                    foreach(var p in pps)
                    {
                        parkplaetze.Add(p.Key, p.Value);
                    }
                }
            }

            return parkplaetze;
        }

    }
}