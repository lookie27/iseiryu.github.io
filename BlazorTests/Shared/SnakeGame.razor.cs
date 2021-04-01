using System;
using System.Linq;
using System.Threading.Tasks;
using BlazorTests.Components.SnakeGame;
using BlazorTests.Models.SnakeGame;
using Excubo.Blazor.Canvas;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTests.Shared {
    public sealed partial class SnakeGame : IDisposable {
        private GameComponent _game;
        private Canvas _canvas;
        private ElementReference _container;

        private int _cellSize;
        
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                var context = await _canvas.GetContext2DAsync();
                var input = new InputComponent();
                _game = new GameComponent(context, input, possibleContentWidth, possibleContentWidth);
                _game.Start();
                
                await _container.FocusAsync();
            }
        }

        public void HandleInput(KeyboardEventArgs e) {
            switch (e.Code) {
                case "ArrowDown":
                    _snake.SetDirection(SnakeDirection.Down);
                    break;
                case "ArrowUp":
                    _snake.SetDirection(SnakeDirection.Up);
                    break;
                case "ArrowLeft":
                    _snake.SetDirection(SnakeDirection.Left);
                    break;
                case "ArrowRight":
                    _snake.SetDirection(SnakeDirection.Right);
                    break;
                case "NumpadAdd":
                    _snake.IncreaseTail();
                    break;
            }

            Console.WriteLine(e.Code + ";" + e.Key);
        }

        public void HandleTouchStart(TouchEventArgs e) {
            _previousTouch = e.Touches.FirstOrDefault();
        }

        public void HandleTouchMove(TouchEventArgs e) {
            if (_previousTouch == null) return;

            var xDiff = _previousTouch.ClientX - e.Touches[0].ClientX;
            var yDiff = _previousTouch.ClientY - e.Touches[0].ClientY;

            // most significant
            if ( Math.Abs(xDiff) > Math.Abs(yDiff) ) {
                _snake.SetDirection(xDiff > 0 ? SnakeDirection.Left : SnakeDirection.Right);
            } else {
                _snake.SetDirection(yDiff > 0 ? SnakeDirection.Up : SnakeDirection.Down);
            }

            _previousTouch = e.Touches[^1];
        }

        public void Dispose() {
            _game.Dispose();
        }
    }
}
