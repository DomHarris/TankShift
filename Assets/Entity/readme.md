#The CollisionEntity Class
This is where most of the magic happens for the tank's character controller. It uses principles from Sebastian Lague's character controller series. In particular, I've used these episodes:
- https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1
- https://www.youtube.com/watch?v=OBtaLCmJexk&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=2
- https://www.youtube.com/watch?v=cwcC2tIKObU&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=4
- https://www.youtube.com/watch?v=kVBABNw1jaE&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=5

It has been slightly modified in these ways:
1. Renamed a bunch of things to follow standard Unity/C# naming conventions
2. Cached some frequently accessed values
3. Uses `Physics2D.RaycastNonAlloc` instead of `Raycast` to avoid allocations
4. Added a ton of comments to describe why certain things are done
5. Added a `PhysicsEntity` class that is used to add forces without interfering with player input