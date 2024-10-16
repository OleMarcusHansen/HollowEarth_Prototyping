using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgeNoiseFilter : INoiseFilter
{
    NoiseSettings settings;
    Noise noise = new Noise();

    public RidgeNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
        this.settings.centre = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            v *= v;
            v *= weight;
            weight = v;

            noiseValue += v;
            noiseValue *= amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);

        noiseValue *= settings.strength;

        //noiseValue = Mathf.Round(noiseValue * 4) / 4f;

        return noiseValue;
    }
}
