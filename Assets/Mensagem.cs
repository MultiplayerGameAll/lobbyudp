using UnityEngine;
using System.Collections;
using System;

public class Mensagem  {
    private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);



    public long instant = (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    public string message;
    public string nick;

}
