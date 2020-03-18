using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager {

    public abstract void Awake();
    public abstract void Start();
    public virtual void Update(float dt) { }
}
