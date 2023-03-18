using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ParkhausManager.Models
{
    public class AutomatenViewModel
    {
        public Ticket Ticket { get; set; }

        public Dauermieter Dauermieter { get; set; }
        public string MieterCode { get; set; }

        public string MieterCodeAusfahrt { get; set; }

        public DateTime Eintrittszeit { get; set; }

        // Ticket ID
        public int? TicketNummer { get; set; }

        public int? TicketNummerAusfahrt { get; set; }

        public EntwertetesTicket EntwertetesTicket { get; set; }
    }

    public class EntwertetesTicket
    {
        public Ticket Ticket { get; set; }

        public Double Kosten { get; set; }

        public DateTime Austrittszeit { get; set; }
    }
}