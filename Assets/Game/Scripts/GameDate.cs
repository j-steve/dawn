using System;

public class GameDate
{
    const int DAYS_PER_SEASON = 90;
    const int SEASONS_PER_YEAR = 4;
    const int DAYS_PER_YEAR = DAYS_PER_SEASON * SEASONS_PER_YEAR;

    int elapsedGameDays {
        get { return _elapsedGameDays; }
        set {
            _elapsedGameDays = value;
            year = value / DAYS_PER_YEAR + 1;
            season = (Season)(value % DAYS_PER_YEAR / DAYS_PER_SEASON);
            day = value % DAYS_PER_SEASON + 1;
        }
    }
    int _elapsedGameDays = 0;

    public int year { get; private set; }
    public Season season { get; private set; }
    public int day { get; private set; } 

    public GameDate(int elapsedGameDays = 0) {
        this.elapsedGameDays = elapsedGameDays;
    }

    public void Increment(int daysToIncrement = 1)
    {
        elapsedGameDays += daysToIncrement;
        //if (_elapsedGameDays == DAYS_PER_SEASON) {
        //    day = 1;
        //    if (_elapsedGameDays == DAYS_PER_YEAR) {
        //        season = 0;
        //        year += 1;
        //    } else {
        //        season = (Season)((int)season + 1);
        //    }
        //} else {
        //    day += 1;
        //}
    }

    override public string ToString()
    {
        return "Y{0} {1} D{2:D2}".Format(year, season, day);
    }
}

public enum Season
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3
}

internal class DisplayAttribute : Attribute
{
}