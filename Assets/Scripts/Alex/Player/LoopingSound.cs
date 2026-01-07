using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;

public class LoopingSound {
    private Transform _attachTransform;
    private EventReference _eventReference;

    private EventInstance _instance;
    private bool _isInitialized;

    public bool IsValid => _isInitialized && _instance.isValid();

    public void Initialize(EventReference eventReference, Transform attachTransform = null) {
        _eventReference = eventReference;
        _attachTransform = attachTransform;

        _instance = RuntimeManager.CreateInstance(_eventReference);

        if (_attachTransform != null) {
            RuntimeManager.AttachInstanceToGameObject(_instance, _attachTransform);
        }

        _isInitialized = true;
    }

    public void Start() {
        if (!IsValid) return;

        _instance.start();
    }

    public void Stop(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT) {
        if (!IsValid) return;

        _instance.stop(stopMode);
    }

    public void SetParameter(string name, float value) {
        if (!IsValid) return;

        _instance.setParameterByName(name, value);
    }

    public void SetParameter(string name, float value, float min, float max) {
        if (!IsValid) return;

        _instance.setParameterByName(name, Mathf.Clamp(value, min, max));
    }

    public void SetParameterNormalized(string name, float normalizedValue) {
        if (!IsValid) return;

        _instance.setParameterByName(name, Mathf.Clamp01(normalizedValue));
    }

    public void Release() {
        if (!_isInitialized) return;

        Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (_instance.isValid()) {
            _instance.release();
        }

        _instance = default;
        _isInitialized = false;
    }

    public void Toggle(bool isEnabled) {
        if (isEnabled) {
            Start();
        } else {
            Stop();
        }
    }
}