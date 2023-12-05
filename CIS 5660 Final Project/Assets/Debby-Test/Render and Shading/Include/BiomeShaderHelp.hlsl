void CheckNormalUp_float(float3 Normal, float Error, out float Out)
{
    // false 
    Out = 0.0; 

    // check how close it can get to (0, 1, 0) up vector 
    float3 Up = (0.0, 1.0, 0.0); 
    float3 ErrorVector = (Error, Error, Error);
    float3 UpperRange = Up + ErrorVector; 
    float3 LowerRange = Up - ErrorVector;

    //if (LowerRange.y <= Normal.y && Normal.y <= UpperRange.y) {
    //    Out = 1.0; 
    //}

    if ((LowerRange.x <= Normal.x) && (Normal.x <= UpperRange.x)) {
        if ((LowerRange.y <= Normal.y) && (Normal.y <= UpperRange.y)) {
            if ((LowerRange.z <= Normal.z) && (Normal.z <= UpperRange.z)) {
                Out = 1.0; 
            }
        }
    }
}

void CheckIfGrass_float(float3 Normal, float Error, float Height, float HeightThreshold, out float Out)
{
    // false 
    Out = 0.0;

    // check how close it can get to (0, 1, 0) up vector 
    float3 Up = (0.0, 1.0, 0.0);
    float3 ErrorVector = (Error, Error, Error);
    float3 UpperRange = Up + ErrorVector;
    float3 LowerRange = Up - ErrorVector;

    if (Height >= HeightThreshold) {
        if (LowerRange.y <= Normal.y && Normal.y <= UpperRange.y) {
            Out = 1.0;
        }
    }
}