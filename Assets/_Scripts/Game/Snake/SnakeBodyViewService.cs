using System;
using UnityEngine;
using Zenject;

public class SnakeBodyViewService : ILateTickable, IDisposable
{
    private readonly SnakePartsCollectorService _partsCollectorService;
    private readonly LineRenderer _snakeLineRenderer;
    private bool _isHeadCollided;

    public SnakeBodyViewService(SnakePartsCollectorService snakePartsCollectorService, LineRenderer snakeLineRenderer)
    {
        (_partsCollectorService, _snakeLineRenderer) = (snakePartsCollectorService, snakeLineRenderer);
        _snakeLineRenderer.positionCount = 1;
        _partsCollectorService.OnTailCreated += HandleTailCreated;
        _partsCollectorService.OnHeadCollided += HandleHeadCollided;
    }
    private void HandleHeadCollided()
    {
        _partsCollectorService.OnHeadCollided -= HandleHeadCollided;
        _partsCollectorService.OnTailCreated -= HandleTailCreated;
        _isHeadCollided = true;
    }

    private void HandleTailCreated()
    {
        _snakeLineRenderer.positionCount++;
    }

    public void LateTick()
    {
        if (_isHeadCollided)
        {
            return;
        }

        for (int i = 0; i < _partsCollectorService.SnakeParts.Count; i++)
        {
            _snakeLineRenderer.SetPosition(i, _partsCollectorService.SnakeParts[i].PartTransform.localPosition);
        }
    }
    public void Dispose()
    {
        _partsCollectorService.OnTailCreated -= HandleTailCreated;
        _partsCollectorService.OnHeadCollided -= HandleHeadCollided;
    }
}
