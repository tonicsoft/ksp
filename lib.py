import math;

class Vector2:
        def __init__(self, x, y):
            self.x = x;
            self.y = y;
            
        def __str__(self):
            return "Vector2(" + str(self.x) + "," + str(self.y) + ")";

        def dot(self, other):
            return self.x * other.x + self.y * other.y;

        def squareMagnitude(self):
            return self.x * self.x + self.y * self.y;

        def magnitude(self):
            return math.sqrt(self.squareMagnitude());

        def __mul__(self, other):
            return Vector2(other * self.x, other * self.y);

        def __truediv__(self, other):
            return Vector2(self.x / other, self.y / other);

        def __add__(self, other):
            return Vector2(self.x + other.x, self.y + other.y);

        def __sub__(self, other):
            return Vector2(self.x - other.x, self.y - other.y);

        def unit(self):
            return self / self.magnitude();

Vector2.ZERO = Vector2(0, 0);

class OrbitalState:
    def __init__(self, position, velocity):
        self.position = position;
        self.velocity = velocity;

class OrbitalUtilities:

        def __init__(self, gravitationalConstant, planetMass):
            self.G = gravitationalConstant;
            self.M = planetMass;
            self.μ = self.G * self.M;

        def specificOrbitalEnergyS(self, state):
            return self.specificOrbitalEnergy(state.position, state.velocity);
        
        def specificOrbitalEnergy(self, position, velocity):
            kineticEnergy = 0.5 * velocity.squareMagnitude();

            potentialEnergy = - self.μ / position.magnitude();

            return kineticEnergy + potentialEnergy;

        def specificOrbitalEnergyOfCircularOrbit(self, radius):
            kineticEnergy = 0.5 * self.circularOrbitSpeed(radius) * self.circularOrbitSpeed(radius);

            potentialEnergy = - self.μ / radius;

            return kineticEnergy + potentialEnergy;

        def accelerationDueToGravity(self, position):
            return position * -(self.μ / pow(position.squareMagnitude(), 1.5));

        def circularOrbitSpeed(self, radius):
            return math.sqrt(self.μ / radius);

        def eccentricityVector(self, position, velocity):
            return position * (velocity.squareMagnitude() / self.μ - 1/position.magnitude()) - velocity * (position.dot(velocity)/self.μ);

        def eccentricityS(self, state):
            return self.eccentricity(state.position, state.velocity);
        
        def eccentricity(self, position, velocity):
            return self.eccentricityVector(position, velocity).magnitude();

        def semiMajorAxisS(self, state):
            return self.semiMajorAxis(state.position, state.velocity);
        
        def semiMajorAxis(self, position, velocity):
            return -self.μ / (2 * self.specificOrbitalEnergy(position, velocity));

KspGravitationalConstant = 6.67430E-11;
KerbinMass = 5.2915158E22;

KerbinOrbitalUtilities = OrbitalUtilities(KspGravitationalConstant, KerbinMass);

class PathToOrbitSimulator:
    def __init__(self, util):
        self.util = util
        
    
    def simulateTimeStep(self, initialState, thrust, timestep):
        acceleration = lambda r, v : self.util.accelerationDueToGravity(r) + thrust(r, v);
        
        return self.computeRk4Iteration(initialState, timestep, acceleration);
    
    def runSimulation(self, initialPosition, initialVelocity, thrust, timestep, cont):
        currentTime = 0.0;
        currentState = OrbitalState(initialPosition, initialVelocity);
        result = [];
        
        while (cont(currentTime, currentState)):
            result.append(currentState);

            currentState = self.simulateTimeStep(currentState, thrust, timestep);

            currentTime += timestep;

        return result;


    def computeRk4Iteration(self, currentState, h, a):
        r0 = currentState.position;
        v0 = currentState.velocity;
        a0 = a(r0, v0);

        k1r = v0;
        k1v = a0;

        k2r = v0 + (k1v * 0.5 * h);
        k2v = a(r0 + k1r * 0.5 * h, k2r);

        k3r = v0 + k2v * 0.5 * h;
        k3v = a(r0 + k2r * 0.5 * h, k3r);

        k4r = v0 + k3v * h;
        k4v = a(r0 + k3r * h, k4r);

        r1 = r0 + (k1r + k2r * 2 + k3r * 2 + k4r) * (h / 6);
        v1 = v0 + (k1v + k2v * 2 + k3v * 2 + k4v) * (h / 6);

        return OrbitalState(r1, v1);