//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Parkhaus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Parkhaus()
        {
            this.Stockwerk = new HashSet<Stockwerk>();
            this.Tarif = new HashSet<Tarif>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ort { get; set; }
        public Nullable<double> Tagestarif { get; set; }
        public Nullable<double> Monatsmiete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stockwerk> Stockwerk { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tarif> Tarif { get; set; }
    }
}
