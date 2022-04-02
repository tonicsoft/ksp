import matplotlib as mpl
import numpy as np


def plot_states(ax, states, colour):
    ax.set_aspect('equal')
    xs = []
    ys = []
    
    for state in states:
        xs.append(state.position.x)
        ys.append(state.position.y)
        
    ax.plot(xs, ys, colour)
    
def plot_radius(ax, states, colour):
    xs = []
    ys = []
    t = 0
    
    for state in states:
        xs.append(t)
        ys.append(state.position.magnitude())
        t = t +1
    
    ax.plot(xs, ys, colour)
    
def draw_axes(ax):
    ax.axhline(y=0, color='k')
    ax.axvline(x=0, color='k')
    
def plot_ellipse_parameters(ax, states, util, colour, targetOrbitalRadius):
    es = []
    smas = []
    
    for state in states:
        es.append(util.eccentricityS(state))
        smas.append(util.semiMajorAxisS(state))
    ax.plot(es, smas, colour)
    ax.axhline(y=targetOrbitalRadius, color='k')
    ax.axvline(x=0, color='k')
    
def plot_radius_and_e(ax, states, util, colour):
    es = []
    rs = []
    
    for state in states:
        es.append(util.eccentricityS(state))
        rs.append(state.position.magnitude())
    ax.plot(es, rs, colour)
    
    
def plot_acceleration_function(ax, startingR, finalR, accelerationFunction, acceleration):
    Rs = []
    As = []

    for i in range(0,21):
        Rs.append((1/20) * ((20-i)* startingR + i * finalR))
        As.append(accelerationFunction(Rs[-1]))

    ax.axvline(x=startingR, color='k')
    ax.axvline(x=finalR, color='k')
    ax.axhline(y=acceleration, color='k')
    ax.plot(Rs, As, 'green')
