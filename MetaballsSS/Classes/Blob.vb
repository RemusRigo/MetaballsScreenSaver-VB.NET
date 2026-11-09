Imports System

''' <summary>
''' A single wax blob. Movement is a slow random-walk on heading (not a
''' bouncing-ball simulation) so blobs drift and curl the way lava lamp wax
''' does, gently reversing course at the screen edges instead of ricocheting.
'''
''' TUTORIAL: a blob doesn't know anything about "drawing" - it's pure
''' position + velocity state. LavaField.vb reads .X, .Y, and .Radius from
''' each blob every frame to build the visual field. Separating "where is
''' it and how does it move" (this file) from "how does it look" (LavaField
''' + ColorPalette) is a common and useful split in graphics/game code.
''' </summary>
Public Class Blob

    Public Property X As Double
    Public Property Y As Double
    Public ReadOnly Property Radius As Double

    Private _vx As Double
    Private _vy As Double
    Private _screenWidth As Integer
    Private _screenHeight As Integer
    Private ReadOnly _rand As Random
    Private _wanderTimer As Double
    Private _wanderTarget As Double

    Public Sub New(x As Double, y As Double, blobRadius As Double, screenWidth As Integer, screenHeight As Integer, rand As Random, speedMultiplier As Double)
        Me.X = x
        Me.Y = y
        Radius = blobRadius
        _screenWidth = screenWidth
        _screenHeight = screenHeight
        _rand = rand

        Dim speed = Math.Max(screenWidth, screenHeight) * 0.025 * speedMultiplier
        Dim angle = _rand.NextDouble() * Math.PI * 2.0
        _vx = Math.Cos(angle) * speed
        _vy = Math.Sin(angle) * speed
        _wanderTarget = angle
        _wanderTimer = _rand.NextDouble() * 2.0
    End Sub

    Public Sub UpdateBounds(screenWidth As Integer, screenHeight As Integer)
        _screenWidth = screenWidth
        _screenHeight = screenHeight
    End Sub

    ''' <summary>
    ''' Advances the blob's position by one frame. dt ("delta time", in
    ''' seconds) is how much real time passed since the last frame - using it
    ''' instead of a fixed step per call means motion speed stays consistent
    ''' even if the frame rate varies (a slow frame moves the blob further to
    ''' compensate). This is the standard "dt-based" update pattern used
    ''' throughout real-time graphics and games.
    ''' </summary>
    Public Sub Update(dt As Double)
        ' Every couple of seconds, pick a brand-new "target heading" - a
        ' random angle to gently steer toward. Waiting on a timer rather than
        ' rerolling every frame is what makes the path feel like a lazy drift
        ' instead of a jittery random shake.
        _wanderTimer -= dt
        If _wanderTimer <= 0 Then
            _wanderTimer = 1.5 + _rand.NextDouble() * 2.5
            Dim currentHeading = Math.Atan2(_vy, _vx)
            ' New target is the current heading plus a random turn of up to
            ' ~half pi either way - it steers, it doesn't teleport to a random
            ' direction, so blobs never do a sudden U-turn.
            _wanderTarget = currentHeading + (_rand.NextDouble() - 0.5) * (Math.PI * 0.9)
        End If

        ' Speed (how fast) and heading (which way) are tracked separately.
        ' Each frame we nudge the *heading* a little closer to _wanderTarget -
        ' we never touch speed here, so the blob always drifts at a constant
        ' pace, just slowly curving. That gradual turn (rather than snapping
        ' straight to the new heading) is what produces the smooth curling
        ' paths instead of blobs that visibly change direction on a dime.
        Dim speed = Math.Sqrt(_vx * _vx + _vy * _vy)
        Dim heading = Math.Atan2(_vy, _vx)
        Dim diff = NormalizeAngle(_wanderTarget - heading)
        heading += diff * Math.Min(dt * 0.6, 1.0)

        ' Convert the (speed, heading) pair back into (vx, vy) components -
        ' the reverse of the Atan2/Sqrt above - so the position update below
        ' is just simple vector addition.
        _vx = Math.Cos(heading) * speed
        _vy = Math.Sin(heading) * speed

        X += _vx * dt
        Y += _vy * dt

        ' Edge handling: instead of a hard "bounce off the wall like a ball"
        ' rule, we clamp the position to a margin and flip only the velocity
        ' component pointing further out-of-bounds. Combined with resetting
        ' _wanderTarget to match, this reads as the blob "changing its mind"
        ' near the edge rather than physically ricocheting - closer to how
        ' wax drifts back from the glass in a real lava lamp.
        Dim margin = Radius * 0.5
        Dim maxX = Math.Max(_screenWidth - margin, margin)
        Dim maxY = Math.Max(_screenHeight - margin, margin)

        If X < margin Then
            X = margin
            _vx = Math.Abs(_vx)
            _wanderTarget = Math.Atan2(_vy, _vx)
        ElseIf X > maxX Then
            X = maxX
            _vx = -Math.Abs(_vx)
            _wanderTarget = Math.Atan2(_vy, _vx)
        End If

        If Y < margin Then
            Y = margin
            _vy = Math.Abs(_vy)
            _wanderTarget = Math.Atan2(_vy, _vx)
        ElseIf Y > maxY Then
            Y = maxY
            _vy = -Math.Abs(_vy)
            _wanderTarget = Math.Atan2(_vy, _vx)
        End If
    End Sub

    Private Function NormalizeAngle(a As Double) As Double
        Dim result = a
        While result > Math.PI
            result -= Math.PI * 2.0
        End While
        While result < -Math.PI
            result += Math.PI * 2.0
        End While
        Return result
    End Function

End Class
