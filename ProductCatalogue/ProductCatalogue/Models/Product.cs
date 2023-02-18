using System.ComponentModel.DataAnnotations;
namespace ProductCatalogue.Models;

public class Product
{
  public Guid id { get; set; }  
  public string Name { get; set; }
  public string Type { get; set; }
  public decimal Price { get; set; }
}