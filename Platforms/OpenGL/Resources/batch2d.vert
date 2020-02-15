#version 330

uniform mat4 u_matrix;

in vec2 a_position;
in vec2 a_tex;
in vec4 a_color;
in vec3 a_type;

out vec2 v_tex;
out vec4 v_col;
out vec3 v_type;

void main(void)
{
    gl_Position = u_matrix * vec4(a_position, 0.0, 1.0);

    v_tex = a_tex;
    v_col = a_color;
	v_type = a_type;
}