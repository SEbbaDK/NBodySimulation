kernel void affect(__global float * positionVectors, __global float * velocityVectors, __global float * mass, float deltaTime, int vectorCount) {
  int id = get_global_id(0);
  int vectorNumber = id * 3;

  for(int j = 0; j < vectorCount; j++)
  {
    int otherVectorNumber = j * 3;

    if(vectorNumber != otherVectorNumber)
    {
      float abVector[3];

      for(int k = 0; k < 3; k++)
        abVector[k] = positionVectors[otherVectorNumber + k] - positionVectors[vectorNumber + k];

      float magnitude = sqrt( pown(abVector[0], 2) + pown(abVector[1], 2) + pown(abVector[2], 2) );
      float force = ( 0.00000000006674f * ( mass[id] * mass[j] ) / ( magnitude * magnitude ) ) * deltaTime;

      for(int l = 0; l < 3; l++)
        velocityVectors[vectorNumber + l] = velocityVectors[vectorNumber + l] + ( ( abVector[l] / magnitude ) * force );
    }
  }
}
