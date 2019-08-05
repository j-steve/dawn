using UnityEngine;
using System.Collections;

public interface ITileConstruct {
    string Name { get; }
}

public interface ITileCenterConstruct : ITileConstruct { }

