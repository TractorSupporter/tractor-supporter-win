using System.Configuration;
using TractorSupporter.Model;

namespace TractorSupporter.Services.Abstract;

public abstract class CommandFieldDecision
{
    private readonly int _minDistance = int.Parse(ConfigurationManager.AppSettings["MinDistance"] ?? "0");
    //private readonly int _lidarMaxAcceptableError = int.Parse(ConfigurationManager.AppSettings["LidarMaxAcceptableError"] ?? "0");
    //private readonly int _lidarMinConfirmationCount = int.Parse(ConfigurationManager.AppSettings["LidarMinConfirmationCount"] ?? "0");
    public int _lidarMaxAcceptableError;
    public int _lidarMinConfirmationCount;

    public List<Obstacle2D> obstacles;

    public bool MakeDecision(double distanceMeasured, List<(DateTime time, double dist)> distanceTimes, double distance, int validLifetimeMs, int minSignalsCount)
    {
        var currentTime = DateTime.Now;

        distanceTimes.RemoveAll(tim => (currentTime - tim.time).TotalMilliseconds > validLifetimeMs);

        if (distanceMeasured <= distance && distance >= _minDistance)
            distanceTimes.Add((currentTime, distanceMeasured));



        var decision = distanceTimes.Count >= minSignalsCount;
        if (decision)
        {

            var max = distanceTimes.Select(x => x.dist).Max();
            var min = distanceTimes.Select(x => x.dist).Min();

            var valid = max - min < 100;

            return valid;
        }
        else return false;
    }

    public (bool, double) MakeDecision(Dictionary<int, double> newMeasurements, double speed, List<(DateTime time, double dist)> distanceTimes, double distance, int validLifetimeMs, int minSignalsCount)
    {
        var currentTime = DateTime.Now;

        foreach (var obs in obstacles)
        {
            double dt = (currentTime - obs.LastUpdateTime).TotalSeconds;
            obs.Y -= speed * dt;
        }

        List<Obstacle2D> newObstacles = new List<Obstacle2D>();

        foreach (var (angle, measurement) in newMeasurements)
        {
            if (measurement <= 0)
            {
                continue;
            }

            double angleRad = angle * (Math.PI / 180);
            double x = Math.Sin(angleRad) * measurement;
            double y = Math.Cos(angleRad) * measurement;

            Obstacle2D bestObstacle = null;
            double bestDistance = double.MaxValue;

            foreach (Obstacle2D obstacle in obstacles)
            {
                double dx = x - obstacle.X;
                double dy = y - obstacle.Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestObstacle = obstacle;
                }
            }

            if (bestDistance < _lidarMaxAcceptableError && bestObstacle != null)
            {
                bestObstacle.X = x;
                bestObstacle.Y = y;
                bestObstacle.ConfirmCount++;
            }
            else
            {
                newObstacles.Add(new Obstacle2D(x, y, currentTime));
            }
        }

        obstacles.AddRange(newObstacles);

        obstacles.RemoveAll(o => (currentTime - o.LastUpdateTime).TotalSeconds > 1);
        obstacles.RemoveAll(o => o.Y < 0);

        double closestObstacleDistance = double.MaxValue;
        Obstacle2D closestObstacle = null;

        foreach (var obs in obstacles)
        {
            if (obs.ConfirmCount >= _lidarMinConfirmationCount)
            {
                double distanceToObstacle = Math.Sqrt(obs.X * obs.X + obs.Y * obs.Y); 
                if (distanceToObstacle < closestObstacleDistance)
                {
                    closestObstacle = obs;
                    closestObstacleDistance = distanceToObstacle;
                }
            }
        }

        if (closestObstacle != null)
        {
            return (true, closestObstacleDistance / 10);
        }

        return (false, 0);
    }
}
