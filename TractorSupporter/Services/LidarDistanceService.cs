using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model;

namespace TractorSupporter.Services;

public interface ILidarDistanceService
{
    public void setLidarMax(int value);
    public void setLidarMin(int value);
    public void setLidarTime(int value);

    public double FindClosestDistance(Dictionary<double, double> newMeasurements, double speed);
}

public class LidarDistanceService: ILidarDistanceService
{
    //private readonly int _lidarMaxAcceptableError = int.Parse(ConfigurationManager.AppSettings["LidarMaxAcceptableError"] ?? "0");
    //private readonly int _lidarMinConfirmationCount = int.Parse(ConfigurationManager.AppSettings["LidarMinConfirmationCount"] ?? "0");
    //private readonly int _lidarTimeOfMeasurementLife = int.Parse(ConfigurationManager.AppSettings["LidarMinConfirmationCount"] ?? "0");
    public int _lidarMaxAcceptableError;
    public int _lidarMinConfirmationCount;
    public int _lidarTimeOfMeasurementLife;
    public List<Obstacle2D> obstacles = new List<Obstacle2D>();

    public void setLidarMax(int value)
    {
        _lidarMaxAcceptableError = value;
    }

    public void setLidarMin(int value)
    {
        _lidarMinConfirmationCount = value;
    }

    public void setLidarTime(int value)
    {
        _lidarTimeOfMeasurementLife = value;
    }

    public double FindClosestDistance(Dictionary<double, double> newMeasurements, double speed)
    {
        var currentTime = DateTime.Now;

        foreach (var obs in obstacles)
        {
            double dt = (currentTime - obs.LastUpdatedPlaceTime).TotalMilliseconds / 1000;
            obs.Y -= speed * dt;
            obs.LastUpdatedPlaceTime = currentTime;
        }

        List<Obstacle2D> newObstacles = new List<Obstacle2D>();

        foreach (var (angle, measurement) in newMeasurements)
        {
            if (measurement <= 0 || measurement >= 11000)
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
                bestObstacle.LastUpdateTime = currentTime;
            }
            else
            {
                newObstacles.Add(new Obstacle2D(x, y, currentTime, currentTime, currentTime));
            }
        }

        obstacles.AddRange(newObstacles);

        obstacles.RemoveAll(o => (currentTime - o.LastUpdateTime).TotalMilliseconds > _lidarTimeOfMeasurementLife);
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
            return closestObstacleDistance / 10;
        }

        return 0;
    }
}
