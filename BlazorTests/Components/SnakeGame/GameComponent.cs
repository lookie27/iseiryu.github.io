using System;
using System.Threading.Tasks;
using System.Timers;
using BlazorTests.Models.SnakeGame;
using Excubo.Blazor.Canvas.Contexts;

namespace BlazorTests.Components.SnakeGame {
    public sealed class GameComponent : IDisposable {
        private readonly Timer _timer;
        private readonly Context2D _context;
        private readonly InputComponent _input;
        private readonly int _width;
        private readonly int _height;
        private readonly int _cellSize;
        private readonly Egg _egg;
        private readonly Snake _snake;
        private const double _interval = 1000 / 60; //60 FPS
        private bool _paused;
        private bool _gameOver;

        public GameComponent(Context2D context, InputComponent input, int screenWidth, int screenHeight) {
            _context = context;
            _input = input;
            _width = screenWidth;
            _height = screenHeight;
            _cellSize = _width / 25;
            
            _egg = new Egg(_cellSize, _width, _height);
            _snake = new Snake(_cellSize, _width, _height);
            
            _timer = new Timer(_interval);
            _timer.Elapsed += async (_, _) => { await LoopAsync(); };
        }

        public void Start() {
            _paused = false;
            _gameOver = false;
            _timer.Start();
        }
        
        public void Pause() {
            _paused = true;
            _timer.Stop();
        }
        
        private async ValueTask UpdateAsync() {
            if (_snake.Ate(_egg)) {
                _egg.NewLocation();
            }

            _snake.Update();

            if (_snake.IsDead())
                await GameOver();
        }

        private async ValueTask RenderAsync() {
            await using var batch = await _context.CreateBatchAsync();

            await ClearScreenAsync(batch);
            await batch.FillStyleAsync("white");
            await batch.FontAsync("12px serif");
            await batch.FillTextAsync("Score: " + _snake.Tail.Count, _width - 55, 10);

            foreach (var cell in _snake.Tail) {
                await batch.FillRectAsync(cell.X, cell.Y, _cellSize, _cellSize);
            }

            await batch.FillStyleAsync("green");
            await batch.FillRectAsync(_snake.Head.X, _snake.Head.Y, _cellSize, _cellSize);

            await batch.FillStyleAsync("yellow");
            await batch.FillRectAsync(_egg.X, _egg.Y, _cellSize, _cellSize);
        }
        
        private async Task ClearScreenAsync(Batch2D batch) {
            await batch.ClearRectAsync(0, 0, _width, _height);
            await batch.FillStyleAsync("black");
            await batch.FillRectAsync(0, 0, _width, _height);
        }
        
        private async ValueTask GameOver() {
            _timer.Stop();
            _gameOver = true;
            
            await _context.FillStyleAsync("red");
            await _context.FontAsync("48px serif");
            await _context.FillTextAsync("Game Over", _width / 4, _height / 2);
        }
        
        private async ValueTask LoopAsync() {
            if (!_paused && !_gameOver) {
                await UpdateAsync();
                await RenderAsync();
            }
        }

        public void Dispose() {
            _timer.Dispose();
            _context.DisposeAsync();
        }
    }
}
