#version 450

layout(binding = 0) uniform sampler2D u_texture;

layout(location = 0) in vec2 v_tex;
layout(location = 1) in vec4 v_col;
layout(location = 2) in vec3 v_type;

layout(location = 0) out vec4 o_color;

void main()
{
    vec4 color = texture(u_texture, v_tex);
    o_color = 
        v_type.x * color * v_col + 
        v_type.y * color.a * v_col + 
        v_type.z * v_col;
}