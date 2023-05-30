# Mécanique des fluides

## Présentation

On a pour projet de simuler la mécanique des fluides, on a choisi unity comme moteur de développement, car simple d'utilisation avec des colliders déjà fait et un système de rendu déjà présent aussi. Le but était d'implémenter cet algorithme "Smoothed Particle Hydrodynamics".
## Projet

Il est séparé en plusieurs grosses parties qui sont :
### Fluid Manager

Dedans, on y trouve toutes les fonctions qui géraient l'apparition de particule, le rendu graphique de particule, les collisions entre elles et le calcul des voisins.
### Smoothed Particle Hydrodynamics
Tous les calculs pour la mécanique des fluides se trouvent dedans. (pression, density, viscosité etc...)
### Particule
Contient les data pour les particules leurs masses par exemple
### Particule Renderer
class qui permet de voir les particule à l'écran 

## Version

unity 2021.3.18f1
