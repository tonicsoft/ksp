import matplotlib as mpl
import matplotlib.pyplot as plt
import numpy as np

fig = plt.figure()
ax = plt.axes()


file1 = open('position-plot.txt', 'r')
lines = file1.readlines()

xs = []
ys = []

for line in lines:
    if line.strip() == "":
        ax.plot(xs, ys, 'gray')
        xs = []
        ys = []
    else:
        values = line.split(',')

        x  = float(values[0])
        y = float(values [1])
        
        xs.append(x)
        ys.append(y)

file1.close()


ax.axhline(y=0, color='k')
ax.axvline(x=0, color='k')

plt.xlim([-1000000, 1000000])
plt.ylim([-1000000,1000000])

plt.show()
