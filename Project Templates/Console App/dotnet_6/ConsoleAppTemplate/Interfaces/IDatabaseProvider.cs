using System.Collections.Generic;

namespace ConsoleAppTemplate.Interfaces;

public interface IDatabaseProvider
{
    List<string> GetData();
}