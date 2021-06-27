using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRecordManager : MonoBehaviour
{
    [SerializeField] TrackRecord track_record;
    [SerializeField] string track_name;
    [SerializeField] int next_track;

    static GameRecordManager grm;

    void Start()
    {
        grm = this;
    }

    public static int Renewal() //1=>1등 2=>2등 3=>3등 4=>꼴등 0=>실패
    {
        if (!grm) return 0;
        GameDataManager.ComplateTrack(grm.next_track);
        grm.track_record.Load(grm.track_name, out TrackRecordNode trackRN);
        if (trackRN==null)
        {
            var tRN = new TrackRecordNode();
            tRN.records = new string[3]
            {
                "00:00.000",
                "00:00.000",
                "00:00.000"
            };
            tRN.records[0] = Timer.GetTime();
            grm.track_record.Save(grm.track_name, tRN);
            return 1;
        }

        var record_list = new LinkedList<string>(trackRN.records);
        var record = record_list.First;
        for(int i=1; i<4; i++)
        {
            if(record.Value == "00:00.000" || string.Compare(record.Value, Timer.GetTime())>=0)
            {
                if (string.Compare(record.Value, Timer.GetTime()) != 0)
                {
                    record_list.AddBefore(record, Timer.GetTime());
                    record_list.RemoveLast();


                    int c = 0;
                    foreach (var re in record_list)
                    {
                        trackRN.records[c] = re;
                        ++c;
                    }

                    grm.track_record.Save(grm.track_name, trackRN);
                }
                return i;
            }
            record = record.Next;
        }
        return 4;
    }
}
