﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ParkhausManager
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ParkhausEntities : DbContext
    {
        public ParkhausEntities()
            : base("name=ParkhausEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Parkhaus> Parkhaus { get; set; }
        public virtual DbSet<Stockwerk> Stockwerk { get; set; }
        public virtual DbSet<Tarif> Tarif { get; set; }
        public virtual DbSet<Dauermieter> Dauermieter { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<Zahlung> Zahlung { get; set; }
    }
}
