using System;
using System.Collections.Generic;

namespace IOrder.Communication.Request;

public class UpdateCategoryPositionsRequestDto
{
    public IList<CategoryPositionDto> Positions { get; set; } = [];
}

public class CategoryPositionDto
{
    public Guid CategoryId { get; set; }
    public int Position { get; set; }
}
