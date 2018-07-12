﻿using Dotnet.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.Dapper.Dapper
{
    public interface IDapperRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
    {

    }
}
