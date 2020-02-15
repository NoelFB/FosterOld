#version 450

layout(binding = 0) uniform WorldUniform {
    mat4 u_matrix;
} world;

layout(location = 0) in vec2 a_position;
layout(location = 1) in vec2 a_tex;
layout(location = 2) in vec4 a_color;
layout(location = 3) in vec3 a_type;

layout(location = 0) out vec2 v_tex;
layout(location = 1) out vec4 v_col;
layout(location = 2) out vec3 v_type;

void main()
{
    gl_Position = world.u_matrix * vec4(a_position, 0.0, 1.0);

    v_tex = a_tex;
    v_col = a_color;
	v_type = a_type;
}