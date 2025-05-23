PGDMP          
            }            enrollment_db    17.2    17.2     ,           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                           false            -           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                           false            .           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                           false            /           1262    20079    enrollment_db    DATABASE     �   CREATE DATABASE enrollment_db WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_Philippines.1252';
    DROP DATABASE enrollment_db;
                     postgres    false            �            1259    20118    faculty    TABLE     $  CREATE TABLE public.faculty (
    id integer NOT NULL,
    fullname character varying(150) NOT NULL,
    username character varying(100) NOT NULL,
    password text NOT NULL,
    isprofessor boolean DEFAULT false,
    isadmin boolean DEFAULT false,
    isprogramhead boolean DEFAULT false
);
    DROP TABLE public.faculty;
       public         heap r       postgres    false            �            1259    20117    faculty_id_seq    SEQUENCE     �   CREATE SEQUENCE public.faculty_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 %   DROP SEQUENCE public.faculty_id_seq;
       public               postgres    false    219            0           0    0    faculty_id_seq    SEQUENCE OWNED BY     A   ALTER SEQUENCE public.faculty_id_seq OWNED BY public.faculty.id;
          public               postgres    false    218            �            1259    20108    student    TABLE     �  CREATE TABLE public.student (
    stud_code character varying(50) NOT NULL,
    stud_lname character varying(100) NOT NULL,
    stud_fname character varying(100) NOT NULL,
    stud_mname character varying(100),
    stud_bod date NOT NULL,
    stud_contact character varying(20) NOT NULL,
    stud_email character varying(100) NOT NULL,
    stud_address text NOT NULL,
    stud_password text NOT NULL
);
    DROP TABLE public.student;
       public         heap r       postgres    false            �           2604    20121 
   faculty id    DEFAULT     h   ALTER TABLE ONLY public.faculty ALTER COLUMN id SET DEFAULT nextval('public.faculty_id_seq'::regclass);
 9   ALTER TABLE public.faculty ALTER COLUMN id DROP DEFAULT;
       public               postgres    false    218    219    219            )          0    20118    faculty 
   TABLE DATA           h   COPY public.faculty (id, fullname, username, password, isprofessor, isadmin, isprogramhead) FROM stdin;
    public               postgres    false    219   �       '          0    20108    student 
   TABLE DATA           �   COPY public.student (stud_code, stud_lname, stud_fname, stud_mname, stud_bod, stud_contact, stud_email, stud_address, stud_password) FROM stdin;
    public               postgres    false    217   m       1           0    0    faculty_id_seq    SEQUENCE SET     <   SELECT pg_catalog.setval('public.faculty_id_seq', 3, true);
          public               postgres    false    218            �           2606    20128    faculty faculty_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public.faculty
    ADD CONSTRAINT faculty_pkey PRIMARY KEY (id);
 >   ALTER TABLE ONLY public.faculty DROP CONSTRAINT faculty_pkey;
       public                 postgres    false    219            �           2606    20130    faculty faculty_username_key 
   CONSTRAINT     [   ALTER TABLE ONLY public.faculty
    ADD CONSTRAINT faculty_username_key UNIQUE (username);
 F   ALTER TABLE ONLY public.faculty DROP CONSTRAINT faculty_username_key;
       public                 postgres    false    219            �           2606    20114    student student_pkey 
   CONSTRAINT     Y   ALTER TABLE ONLY public.student
    ADD CONSTRAINT student_pkey PRIMARY KEY (stud_code);
 >   ALTER TABLE ONLY public.student DROP CONSTRAINT student_pkey;
       public                 postgres    false    217            �           2606    20116    student student_stud_email_key 
   CONSTRAINT     _   ALTER TABLE ONLY public.student
    ADD CONSTRAINT student_stud_email_key UNIQUE (stud_email);
 H   ALTER TABLE ONLY public.student DROP CONSTRAINT student_stud_email_key;
       public                 postgres    false    217            )   �   x��ϻ�0����Ut`n8L�Xb�Ԩ�˟�U������g��|M�ݎ���Rn�>kk�������0~L�ϛ�ܳ<-���U<��j���|F�}�L�|�d�� (P$�X���;4�R�d脵N�%���cOt��'�C��K�`����6@��Ư\Q      '   �   x�M��N�@���)\��8�@;[@Z�m$m���+���X����Orrv'�����Bi����EaJ�	�&�0���t��4���QU$oj�u�j������߈���爡���w՚���h�K~+e�/�A����'S]������+��i,��h�û0�ld�*|Π�)�J���3��!�N�iXq��,��}޵4��D�Inʧ}7��$�l�WbY���P�     