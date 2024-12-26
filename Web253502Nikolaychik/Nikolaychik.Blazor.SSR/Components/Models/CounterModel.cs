using System.ComponentModel.DataAnnotations;
namespace Nikolaychik.Blazor.SSR.Components.Models
{
    public class CounterModel
    {
        [Range(1, 10, ErrorMessage = "Please enter a number between 1 and 10.")]
        public int CounterValue { get; set; }
    }
}
