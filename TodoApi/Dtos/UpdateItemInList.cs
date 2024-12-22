using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos;

public class UpdateItemInList
{
    public required string Description { get; set; }
    public bool IsComplete { get; set; }
}