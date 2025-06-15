using System;
using System.Collections.Generic;
using System.Text;

using Supabase.Postgrest.Attributes;

public interface IBaseModel
{

    [PrimaryKey("id", false)] // false indicates this is not auto-incrementing
    Guid Id { get; set; }
}

