using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entiities
{
    public class Autor  /*esta clase tiene que heradad de IValidatableObject si quiere implementar la validacion a nivel del modelo*/
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        //Colecctions
        public List<AutorLibro> AutoresLibros { get; set; }

        //esto es para crear validaciones al nivel del modelo en cuestion.
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (!string.IsNullOrEmpty(Nombre))
        //    {
        //        var primeraLetra = Nombre[0].ToString();
        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra de ser mayúscula",
        //                new string[] {nameof(Nombre)});
        //        }
        //    }
        //}
    }
}
