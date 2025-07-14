using System;
using System.Collections.Generic;
using UnityEngine;

public class SessionSummaryResponse
{
    public string session_id { get; set; }
    public string user_id { get; set; }
    public DateTime start_time { get; set; }
    public DateTime end_time { get; set; }
    public double duration_minutes { get; set; }
    public string current_phase { get; set; }
    public List<string> primary_emotions { get; set; }
    public List<string> emotion_progression { get; set; }
    public string summary { get; set; }
    public List<string> recommendations { get; set; }
    public SessionCharacteristics session_characteristics { get; set; }
}

public class SessionCharacteristics
{

}