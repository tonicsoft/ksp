from mpl_toolkits import mplot3d

import matplotlib as mpl
import matplotlib.pyplot as plt
import numpy as np

fig = plt.figure()
ax = plt.axes(projection='3d')


file1 = open('plot.txt', 'r')
lines = file1.readlines()

smas = []
es = []
rs = []
nonsense = False

for line in lines:
    if line.strip() == "":
        ax.plot3D(smas, es, rs, 'gray')
        smas = []
        es = []
        rs = []
        nonsense = False
    else:
        values = line.split(',')

        a  = float(values[0])
        e = float(values [1])
        r = float(values [2])

        if e < 0 or e > 1:
            nonsense = True

        if not nonsense:
            smas.append(a)
            es.append(e)
            rs.append(r)

file1.close()


plt.xlim([0, 1000000])
plt.ylim([0,1])
ax.set_zlim(0, 1000000)

plt.show()
