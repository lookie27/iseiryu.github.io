using System;
using System.Collections.Generic;

namespace BlazorTests.Models.SnakeGame {
    public sealed class Snake {
        private readonly int _size;
        private readonly int _xLimit;
        private readonly int _yLimit;
        private readonly int _tolerance;
        private SnakeDirection _currentDirection;

        private double _xSpeed,
                       _ySpeed;

        private readonly double _speed;

        public Snake(int size, int fieldWidth, int fieldHeight) {
            _size = size;
            _speed = 1;
            _xLimit = fieldWidth - size;
            _yLimit = fieldHeight - size;
            _tolerance = _size - 5;
        }

        public Cell Head => Tail[^1];
        public List<Cell> Tail { get; } = new() { new Cell(0, 0) };

        public void Update() {
            for (var i = 0; i < Tail.Count - 1; i++)
                Tail[i] = Tail[i + 1];

            if(Tail.Count > 1)
                Tail[^1] = new Cell(Head.X, Head.Y);
            
            Head.X += _xSpeed;
            Head.Y += _ySpeed;
            
            if (Head.X > _xLimit)
                Head.X = 0;
            else if (Head.X < 0)
                Head.X = _xLimit;
            else if (Head.Y > _yLimit)
                Head.Y = 0;
            else if (Head.Y < 0)
                Head.Y = _yLimit;
        }

        public void SetDirection(SnakeDirection snakeDirection) {
            if (snakeDirection == _currentDirection)
                return;
            
            switch (snakeDirection) {
                case SnakeDirection.Up:
                    _xSpeed = 0 * _speed;
                    _ySpeed = -1 * _speed;
                    break;
                case SnakeDirection.Down:
                    _xSpeed = 0 * _speed;
                    _ySpeed = 1 * _speed;
                    break;
                case SnakeDirection.Left:
                    _xSpeed = -1 * _speed;
                    _ySpeed = 0 * _speed;
                    break;
                case SnakeDirection.Right:
                    _xSpeed = 1 * _speed;
                    _ySpeed = 0 * _speed;
                    break;
            }

            _currentDirection = snakeDirection;
        }

        public bool Ate(Egg egg) {
            if (Math.Abs(egg.X - Head.X) < _tolerance && 
                Math.Abs(egg.Y - Head.Y) < _tolerance) {
                
                IncreaseTail();
                return true;
            }

            return false;
        }

        public bool IsDead() {
            for (var i = 0; i < Tail.Count - 1; i++)
                if (Head.X == Tail[i].X && Head.Y == Tail[i].Y)
                    return true;

            return false;
        }

        public void IncreaseTail() {
            Tail.Add(new Cell(Head.X - _size * _xSpeed, 
                              Head.Y - _size * _ySpeed));
        }
    }
}
