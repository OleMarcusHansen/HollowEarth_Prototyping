using UnityEngine;

public class NatureFilter
{
    NatureSettings settings;
    Noise noise = new Noise();

    public NatureFilter(NatureSettings settings)
    {
        this.settings = settings;
        this.settings.centre = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
    }

    public bool Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float altitude = point.magnitude;
        /*if (altitude < settings.minAltitude || altitude > settings.maxAltitude)
        {
            return false;
        }
        {*/
            float maxDistance;
            if (altitude > settings.optAltitude)
            {
                maxDistance = settings.maxAltitude - settings.optAltitude;
            }
            else
            {
                maxDistance = settings.optAltitude - settings.minAltitude;
            }
            float spread = 1f;
            noiseValue += 1f - Mathf.Abs(altitude - settings.optAltitude) / (maxDistance * spread);
        //}

        float latitude = Mathf.Abs(90 - Vector3.Angle(Vector3.up, point));
        /*if (latitude < settings.minLatitude || latitude > settings.maxLatitude)
        {
            return false;
        }
        else
        {*/
            //maxDistance;
            if (latitude > settings.optLatitude)
            {
                maxDistance = settings.maxLatitude - settings.optLatitude;
            }
            else
            {
                maxDistance = settings.optLatitude - settings.minLatitude;
            }
            //float spread = 1f;
            noiseValue += 1f - Mathf.Abs(latitude - settings.optLatitude) / (maxDistance * spread);
        //}

        noiseValue += (noise.Evaluate(point / (50f * settings.noiseScale) + settings.centre) + 1f) * .5f;

        //noiseValue += settings.density;
        //noiseValue += settings.density;

        //make average of alt, lat, noise
        noiseValue /= 3f;

        if (noiseValue > Random.Range(settings.minValue, settings.minValue + 1f - settings.density))
        {
            return true;
        }

        return false;

        /*
        float noiseValue = 0;
        bool noiseBool = true;

        noiseValue = noise.Evaluate(point / 50 + settings.centre);
        
        if (noiseValue < settings.minValue)
        {
            noiseBool = false;
            return noiseBool;
        }

        if (Random.Range(0f, 1f) > settings.density)
        {
            noiseBool = false;
            return noiseBool;
        }

        float altitude = point.magnitude;
        if (altitude < settings.minAltitude || altitude > settings.maxAltitude)
        {
            noiseBool = false;
            return noiseBool;
        }

        float latitude = Mathf.Abs(90 - Vector3.Angle(Vector3.up, point));
        if (latitude < settings.minLatitude || latitude > settings.maxLatitude)
        {
            noiseBool = false;
            return noiseBool;
        }

        return noiseBool;
        */
    }
}
