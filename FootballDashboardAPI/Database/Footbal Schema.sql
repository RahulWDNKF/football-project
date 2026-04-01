-- Company Profile Table
CREATE TABLE IF NOT EXISTS company_profile (
    id INT PRIMARY KEY DEFAULT 1 CHECK (id = 1),
    company_name VARCHAR(255) NOT NULL,
    short_name VARCHAR(100),
    tagline VARCHAR(255),
    description TEXT,
    founded_year INT,
    logo_url VARCHAR(500),
    primary_color VARCHAR(50),
    email VARCHAR(255) NOT NULL,
    phone_number VARCHAR(50),
    alternate_phone VARCHAR(50),
    address_line1 VARCHAR(255),
    address_line2 VARCHAR(255),
    area_locality VARCHAR(255),
    city VARCHAR(100),
    district VARCHAR(100),
    state VARCHAR(100),
    country VARCHAR(100),
    postal_code VARCHAR(20),
    organization_type VARCHAR(100),
    sport_type VARCHAR(100),
    facebook_url VARCHAR(255),
    instagram_url VARCHAR(255),
    twitter_url VARCHAR(255),
    linkedin_url VARCHAR(255),
    youtube_url VARCHAR(255)
);
--
-- PostgreSQL database dump
--

\restrict zg2ktAFn97x6FCe9EoHhxHaCFRtJbPcU5QfKaKDS42nQhFZnSIJn58G3LVboqQP

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

-- Started on 2026-03-27 14:39:26

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 7 (class 2615 OID 19169)
-- Name: auth; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA auth;


ALTER SCHEMA auth OWNER TO postgres;

--
-- TOC entry 6 (class 2615 OID 18627)
-- Name: stf; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA stf;


ALTER SCHEMA stf OWNER TO postgres;

--
-- TOC entry 257 (class 1255 OID 18921)
-- Name: fn_users_delete(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_delete(p_id bigint) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM users
    WHERE id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION public.fn_users_delete(p_id bigint) OWNER TO postgres;

--
-- TOC entry 319 (class 1255 OID 18918)
-- Name: fn_users_email_exists(character varying, bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_email_exists(p_email character varying, p_exclude_user_id bigint DEFAULT NULL::bigint) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM users
    WHERE email = p_email
      AND (p_exclude_user_id IS NULL OR id <> p_exclude_user_id);
$$;


ALTER FUNCTION public.fn_users_email_exists(p_email character varying, p_exclude_user_id bigint) OWNER TO postgres;

--
-- TOC entry 291 (class 1255 OID 18917)
-- Name: fn_users_exists(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_exists(p_id bigint) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM users
    WHERE id = p_id;
$$;


ALTER FUNCTION public.fn_users_exists(p_id bigint) OWNER TO postgres;

--
-- TOC entry 285 (class 1255 OID 18914)
-- Name: fn_users_get_all(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_get_all() RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        id,
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at,
        updated_at
    FROM users
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION public.fn_users_get_all() OWNER TO postgres;

--
-- TOC entry 376 (class 1255 OID 18916)
-- Name: fn_users_get_by_email(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_get_by_email(p_email character varying) RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        id,
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at,
        updated_at
    FROM users
    WHERE email = p_email;
$$;


ALTER FUNCTION public.fn_users_get_by_email(p_email character varying) OWNER TO postgres;

--
-- TOC entry 262 (class 1255 OID 18915)
-- Name: fn_users_get_by_id(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_get_by_id(p_id bigint) RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        id,
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at,
        updated_at
    FROM users
    WHERE id = p_id;
$$;


ALTER FUNCTION public.fn_users_get_by_id(p_id bigint) OWNER TO postgres;

--
-- TOC entry 268 (class 1255 OID 18919)
-- Name: fn_users_insert(character varying, character varying, character varying, character varying, timestamp without time zone, character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_created_at timestamp without time zone, p_phone character varying DEFAULT NULL::character varying, p_status boolean DEFAULT true) RETURNS bigint
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_new_id BIGINT;
BEGIN
    INSERT INTO users (
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at
    )
    VALUES (
        p_name,
        p_email,
        p_password,
        p_role,
        p_phone,
        p_status,
        p_created_at
    )
    RETURNING id INTO v_new_id;

    RETURN v_new_id;
END;
$$;


ALTER FUNCTION public.fn_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_created_at timestamp without time zone, p_phone character varying, p_status boolean) OWNER TO postgres;

--
-- TOC entry 349 (class 1255 OID 18920)
-- Name: fn_users_update(bigint, character varying, character varying, character varying, timestamp without time zone, character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_users_update(p_id bigint, p_name character varying, p_email character varying, p_role character varying, p_updated_at timestamp without time zone, p_phone character varying DEFAULT NULL::character varying, p_status boolean DEFAULT NULL::boolean) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE users
    SET
        name       = p_name,
        email      = p_email,
        role       = p_role,
        phone      = p_phone,
        status     = p_status,
        updated_at = p_updated_at
    WHERE id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION public.fn_users_update(p_id bigint, p_name character varying, p_email character varying, p_role character varying, p_updated_at timestamp without time zone, p_phone character varying, p_status boolean) OWNER TO postgres;

--
-- TOC entry 345 (class 1255 OID 18494)
-- Name: sp_players_delete(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_players_delete(p_id bigint) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE affected INT;
BEGIN
    DELETE FROM public.players WHERE id = p_id;
    GET DIAGNOSTICS affected = ROW_COUNT;
    RETURN affected;
END;
$$;


ALTER FUNCTION public.sp_players_delete(p_id bigint) OWNER TO postgres;

--
-- TOC entry 282 (class 1255 OID 18491)
-- Name: sp_players_exists(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_players_exists(p_id bigint) RETURNS integer
    LANGUAGE sql
    AS $$
    SELECT CASE WHEN EXISTS(SELECT 1 FROM public.players WHERE id = p_id) THEN 1 ELSE 0 END;
$$;


ALTER FUNCTION public.sp_players_exists(p_id bigint) OWNER TO postgres;

--
-- TOC entry 253 (class 1255 OID 18489)
-- Name: sp_players_get_all(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_players_get_all() RETURNS TABLE(id bigint, full_name character varying, date_of_birth date, nationality character varying, player_position character varying, preferred_foot character varying, height_cm integer, weight_kg integer, current_club character varying, contract_start date, contract_end date, contract_status character varying, agent_name character varying, agent_contact character varying, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql
    AS $$
    SELECT id, full_name, date_of_birth, nationality, player_position, preferred_foot,
           height_cm, weight_kg, current_club, contract_start, contract_end,
           contract_status, agent_name, agent_contact, created_at, updated_at
    FROM public.players ORDER BY created_at DESC;
$$;


ALTER FUNCTION public.sp_players_get_all() OWNER TO postgres;

--
-- TOC entry 267 (class 1255 OID 18490)
-- Name: sp_players_get_by_id(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_players_get_by_id(p_id bigint) RETURNS TABLE(id bigint, full_name character varying, date_of_birth date, nationality character varying, player_position character varying, preferred_foot character varying, height_cm integer, weight_kg integer, current_club character varying, contract_start date, contract_end date, contract_status character varying, agent_name character varying, agent_contact character varying, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql
    AS $$
    SELECT id, full_name, date_of_birth, nationality, player_position, preferred_foot,
           height_cm, weight_kg, current_club, contract_start, contract_end,
           contract_status, agent_name, agent_contact, created_at, updated_at
    FROM public.players WHERE id = p_id;
$$;


ALTER FUNCTION public.sp_players_get_by_id(p_id bigint) OWNER TO postgres;

--
-- TOC entry 350 (class 1255 OID 18492)
-- Name: sp_players_insert(character varying, date, character varying, character varying, character varying, integer, integer, character varying, date, date, character varying, character varying, character varying, timestamp without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_players_insert(p_full_name character varying, p_date_of_birth date, p_nationality character varying, p_position character varying, p_preferred_foot character varying, p_height_cm integer, p_weight_kg integer, p_current_club character varying, p_contract_start date, p_contract_end date, p_contract_status character varying, p_agent_name character varying, p_agent_contact character varying, p_created_at timestamp without time zone) RETURNS bigint
    LANGUAGE plpgsql
    AS $$
DECLARE v_id BIGINT;
BEGIN
    INSERT INTO public.players (full_name, date_of_birth, nationality, player_position,
        preferred_foot, height_cm, weight_kg, current_club, contract_start, contract_end,
        contract_status, agent_name, agent_contact, created_at)
    VALUES (p_full_name, p_date_of_birth, p_nationality, p_position, p_preferred_foot,
        p_height_cm, p_weight_kg, p_current_club, p_contract_start, p_contract_end,
        p_contract_status, p_agent_name, p_agent_contact, p_created_at)
    RETURNING id INTO v_id;
    RETURN v_id;
END;
$$;


ALTER FUNCTION public.sp_players_insert(p_full_name character varying, p_date_of_birth date, p_nationality character varying, p_position character varying, p_preferred_foot character varying, p_height_cm integer, p_weight_kg integer, p_current_club character varying, p_contract_start date, p_contract_end date, p_contract_status character varying, p_agent_name character varying, p_agent_contact character varying, p_created_at timestamp without time zone) OWNER TO postgres;

--
-- TOC entry 254 (class 1255 OID 18493)
-- Name: sp_players_update(bigint, character varying, date, character varying, character varying, character varying, integer, integer, character varying, date, date, character varying, character varying, character varying, timestamp without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_players_update(p_id bigint, p_full_name character varying, p_date_of_birth date, p_nationality character varying, p_position character varying, p_preferred_foot character varying, p_height_cm integer, p_weight_kg integer, p_current_club character varying, p_contract_start date, p_contract_end date, p_contract_status character varying, p_agent_name character varying, p_agent_contact character varying, p_updated_at timestamp without time zone) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE affected INT;
BEGIN
    UPDATE public.players SET full_name=p_full_name, date_of_birth=p_date_of_birth,
        nationality=p_nationality, player_position=p_position, preferred_foot=p_preferred_foot,
        height_cm=p_height_cm, weight_kg=p_weight_kg, current_club=p_current_club,
        contract_start=p_contract_start, contract_end=p_contract_end,
        contract_status=p_contract_status, agent_name=p_agent_name,
        agent_contact=p_agent_contact, updated_at=p_updated_at
    WHERE id=p_id;
    GET DIAGNOSTICS affected = ROW_COUNT;
    RETURN affected;
END;
$$;


ALTER FUNCTION public.sp_players_update(p_id bigint, p_full_name character varying, p_date_of_birth date, p_nationality character varying, p_position character varying, p_preferred_foot character varying, p_height_cm integer, p_weight_kg integer, p_current_club character varying, p_contract_start date, p_contract_end date, p_contract_status character varying, p_agent_name character varying, p_agent_contact character varying, p_updated_at timestamp without time zone) OWNER TO postgres;

--
-- TOC entry 321 (class 1255 OID 18501)
-- Name: sp_users_delete(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_users_delete(p_id bigint) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE affected INT;
BEGIN
    DELETE FROM public.users WHERE id = p_id;
    GET DIAGNOSTICS affected = ROW_COUNT;
    RETURN affected;
END;
$$;


ALTER FUNCTION public.sp_users_delete(p_id bigint) OWNER TO postgres;

--
-- TOC entry 274 (class 1255 OID 18498)
-- Name: sp_users_email_exists(character varying, bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_users_email_exists(p_email character varying, p_exclude_user_id bigint DEFAULT NULL::bigint) RETURNS integer
    LANGUAGE sql
    AS $$
    SELECT CASE WHEN EXISTS(
        SELECT 1 FROM public.users WHERE email = p_email
        AND (p_exclude_user_id IS NULL OR id <> p_exclude_user_id)
    ) THEN 1 ELSE 0 END;
$$;


ALTER FUNCTION public.sp_users_email_exists(p_email character varying, p_exclude_user_id bigint) OWNER TO postgres;

--
-- TOC entry 348 (class 1255 OID 18497)
-- Name: sp_users_exists(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_users_exists(p_id bigint) RETURNS integer
    LANGUAGE sql
    AS $$
    SELECT CASE WHEN EXISTS(SELECT 1 FROM public.users WHERE id = p_id) THEN 1 ELSE 0 END;
$$;


ALTER FUNCTION public.sp_users_exists(p_id bigint) OWNER TO postgres;

--
-- TOC entry 341 (class 1255 OID 18626)
-- Name: sp_users_get_all(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_users_get_all() RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        u.id,
        u.name,
        u.email,
        u.password,
        u.role,
        u.phone,
        u.status,
        u.created_at,
        u.updated_at
    FROM users u
    ORDER BY u.created_at DESC;
END;
$$;


ALTER FUNCTION public.sp_users_get_all() OWNER TO postgres;

--
-- TOC entry 288 (class 1255 OID 18496)
-- Name: sp_users_get_by_id(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_users_get_by_id(p_id bigint) RETURNS TABLE(id bigint, user_name character varying, email character varying, user_password character varying, user_role character varying, phone character varying, is_active boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql
    AS $$
    SELECT id, user_name, email, user_password, user_role, phone, is_active, created_at, updated_at
    FROM public.users WHERE id = p_id;
$$;


ALTER FUNCTION public.sp_users_get_by_id(p_id bigint) OWNER TO postgres;

--
-- TOC entry 368 (class 1255 OID 18499)
-- Name: sp_users_insert(character varying, character varying, character varying, character varying, character varying, boolean, timestamp without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_phone character varying, p_status boolean, p_created_at timestamp without time zone) RETURNS bigint
    LANGUAGE plpgsql
    AS $$
DECLARE v_id BIGINT;
BEGIN
    INSERT INTO public.users (user_name, email, user_password, user_role, phone, is_active, created_at)
    VALUES (p_name, p_email, p_password, p_role, p_phone, p_status, p_created_at)
    RETURNING id INTO v_id;
    RETURN v_id;
END;
$$;


ALTER FUNCTION public.sp_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_phone character varying, p_status boolean, p_created_at timestamp without time zone) OWNER TO postgres;

--
-- TOC entry 344 (class 1255 OID 18500)
-- Name: sp_users_update(bigint, character varying, character varying, character varying, character varying, boolean, timestamp without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_users_update(p_id bigint, p_name character varying, p_email character varying, p_role character varying, p_phone character varying, p_status boolean, p_updated_at timestamp without time zone) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE affected INT;
BEGIN
    UPDATE public.users SET user_name=p_name, email=p_email, user_role=p_role,
        phone=p_phone, is_active=p_status, updated_at=p_updated_at WHERE id=p_id;
    GET DIAGNOSTICS affected = ROW_COUNT;
    RETURN affected;
END;
$$;


ALTER FUNCTION public.sp_users_update(p_id bigint, p_name character varying, p_email character varying, p_role character varying, p_phone character varying, p_status boolean, p_updated_at timestamp without time zone) OWNER TO postgres;

--
-- TOC entry 331 (class 1255 OID 18905)
-- Name: fn_club_contacts_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_club_contacts_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.club_contacts
    WHERE club_contact_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_club_contacts_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 307 (class 1255 OID 18902)
-- Name: fn_club_contacts_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_club_contacts_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.club_contacts
    WHERE club_contact_id = p_id;
$$;


ALTER FUNCTION stf.fn_club_contacts_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 251 (class 1255 OID 18899)
-- Name: fn_club_contacts_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_club_contacts_get_all() RETURNS TABLE(club_contact_id character varying, club_id character varying, contact_name character varying, role_name character varying, email character varying, phone character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        club_contact_id,
        club_id,
        contact_name,
        role_name,
        email,
        phone,
        created_at
    FROM stf.club_contacts
    ORDER BY contact_name;
$$;


ALTER FUNCTION stf.fn_club_contacts_get_all() OWNER TO postgres;

--
-- TOC entry 264 (class 1255 OID 18901)
-- Name: fn_club_contacts_get_by_club_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_club_contacts_get_by_club_id(p_club_id character varying) RETURNS TABLE(club_contact_id character varying, club_id character varying, contact_name character varying, role_name character varying, email character varying, phone character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        club_contact_id,
        club_id,
        contact_name,
        role_name,
        email,
        phone,
        created_at
    FROM stf.club_contacts
    WHERE club_id = p_club_id
    ORDER BY contact_name;
$$;


ALTER FUNCTION stf.fn_club_contacts_get_by_club_id(p_club_id character varying) OWNER TO postgres;

--
-- TOC entry 337 (class 1255 OID 18900)
-- Name: fn_club_contacts_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_club_contacts_get_by_id(p_id character varying) RETURNS TABLE(club_contact_id character varying, club_id character varying, contact_name character varying, role_name character varying, email character varying, phone character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        club_contact_id,
        club_id,
        contact_name,
        role_name,
        email,
        phone,
        created_at
    FROM stf.club_contacts
    WHERE club_contact_id = p_id;
$$;


ALTER FUNCTION stf.fn_club_contacts_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 259 (class 1255 OID 18903)
-- Name: fn_club_contacts_insert(character varying, character varying, character varying, character varying, timestamp without time zone, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_club_contacts_insert(p_club_contact_id character varying, p_club_id character varying, p_contact_name character varying, p_role_name character varying, p_created_at timestamp without time zone, p_email character varying DEFAULT NULL::character varying, p_phone character varying DEFAULT NULL::character varying) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.club_contacts (
        club_contact_id,
        club_id,
        contact_name,
        role_name,
        email,
        phone,
        created_at
    )
    VALUES (
        p_club_contact_id,
        p_club_id,
        p_contact_name,
        p_role_name,
        p_email,
        p_phone,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_club_contacts_insert(p_club_contact_id character varying, p_club_id character varying, p_contact_name character varying, p_role_name character varying, p_created_at timestamp without time zone, p_email character varying, p_phone character varying) OWNER TO postgres;

--
-- TOC entry 295 (class 1255 OID 18904)
-- Name: fn_club_contacts_update(character varying, character varying, character varying, character varying, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_club_contacts_update(p_club_contact_id character varying, p_club_id character varying, p_contact_name character varying, p_role_name character varying, p_email character varying DEFAULT NULL::character varying, p_phone character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.club_contacts
    SET
        club_id      = p_club_id,
        contact_name = p_contact_name,
        role_name    = p_role_name,
        email        = p_email,
        phone        = p_phone
    WHERE club_contact_id = p_club_contact_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_club_contacts_update(p_club_contact_id character varying, p_club_id character varying, p_contact_name character varying, p_role_name character varying, p_email character varying, p_phone character varying) OWNER TO postgres;

--
-- TOC entry 284 (class 1255 OID 18898)
-- Name: fn_clubs_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_clubs_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.clubs
    WHERE club_id = p_id;
 
    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_clubs_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 366 (class 1255 OID 18894)
-- Name: fn_clubs_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_clubs_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.clubs
    WHERE club_id = p_id;
$$;


ALTER FUNCTION stf.fn_clubs_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 305 (class 1255 OID 18892)
-- Name: fn_clubs_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_clubs_get_all() RETURNS TABLE(club_id character varying, club_name character varying, country character varying, address_line character varying, logo_url character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        club_id,
        club_name,
        country,
        address_line,
        logo_url,
        created_at
    FROM stf.clubs
    ORDER BY club_name;
$$;


ALTER FUNCTION stf.fn_clubs_get_all() OWNER TO postgres;

--
-- TOC entry 297 (class 1255 OID 18893)
-- Name: fn_clubs_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_clubs_get_by_id(p_id character varying) RETURNS TABLE(club_id character varying, club_name character varying, country character varying, address_line character varying, logo_url character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        club_id,
        club_name,
        country,
        address_line,
        logo_url,
        created_at
    FROM stf.clubs
    WHERE club_id = p_id;
$$;


ALTER FUNCTION stf.fn_clubs_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 320 (class 1255 OID 18896)
-- Name: fn_clubs_insert(character varying, character varying, character varying, timestamp without time zone, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_clubs_insert(p_club_id character varying, p_club_name character varying, p_country character varying, p_created_at timestamp without time zone, p_address_line character varying DEFAULT NULL::character varying, p_logo_url character varying DEFAULT NULL::character varying) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.clubs (
        club_id,
        club_name,
        country,
        address_line,
        logo_url,
        created_at
    )
    VALUES (
        p_club_id,
        p_club_name,
        p_country,
        p_address_line,
        p_logo_url,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_clubs_insert(p_club_id character varying, p_club_name character varying, p_country character varying, p_created_at timestamp without time zone, p_address_line character varying, p_logo_url character varying) OWNER TO postgres;

--
-- TOC entry 263 (class 1255 OID 18895)
-- Name: fn_clubs_name_exists(character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_clubs_name_exists(p_club_name character varying, p_exclude_club_id character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.clubs
    WHERE club_name = p_club_name
      AND (p_exclude_club_id IS NULL OR club_id <> p_exclude_club_id);
$$;


ALTER FUNCTION stf.fn_clubs_name_exists(p_club_name character varying, p_exclude_club_id character varying) OWNER TO postgres;

--
-- TOC entry 372 (class 1255 OID 18897)
-- Name: fn_clubs_update(character varying, character varying, character varying, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_clubs_update(p_club_id character varying, p_club_name character varying, p_country character varying, p_address_line character varying DEFAULT NULL::character varying, p_logo_url character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.clubs
    SET
        club_name    = p_club_name,
        country      = p_country,
        address_line = p_address_line,
        logo_url     = p_logo_url
    WHERE club_id = p_club_id;
 
    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_clubs_update(p_club_id character varying, p_club_name character varying, p_country character varying, p_address_line character varying, p_logo_url character varying) OWNER TO postgres;

--
-- TOC entry 353 (class 1255 OID 19057)
-- Name: fn_documents_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_documents_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.documents
    WHERE document_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_documents_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 329 (class 1255 OID 19054)
-- Name: fn_documents_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_documents_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.documents
    WHERE document_id = p_id;
$$;


ALTER FUNCTION stf.fn_documents_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 255 (class 1255 OID 19052)
-- Name: fn_documents_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_documents_get_all() RETURNS TABLE(document_id character varying, player_id character varying, club_id character varying, document_name character varying, document_type character varying, document_date timestamp without time zone, file_size_label character varying, file_data bytea, file_extension character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        document_id,
        player_id,
        club_id,
        document_name,
        document_type,
        document_date,
        file_size_label,
        file_data,
        file_extension,
        created_at
    FROM stf.documents
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_documents_get_all() OWNER TO postgres;

--
-- TOC entry 271 (class 1255 OID 19053)
-- Name: fn_documents_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_documents_get_by_id(p_id character varying) RETURNS TABLE(document_id character varying, player_id character varying, club_id character varying, document_name character varying, document_type character varying, document_date timestamp without time zone, file_size_label character varying, file_data bytea, file_extension character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        document_id,
        player_id,
        club_id,
        document_name,
        document_type,
        document_date,
        file_size_label,
        file_data,
        file_extension,
        created_at
    FROM stf.documents
    WHERE document_id = p_id;
$$;


ALTER FUNCTION stf.fn_documents_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 269 (class 1255 OID 19055)
-- Name: fn_documents_insert(character varying, character varying, character varying, timestamp without time zone, timestamp without time zone, bytea, character varying, character varying, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_documents_insert(p_document_id character varying, p_document_name character varying, p_document_type character varying, p_document_date timestamp without time zone, p_created_at timestamp without time zone, p_file_data bytea DEFAULT NULL::bytea, p_player_id character varying DEFAULT NULL::character varying, p_club_id character varying DEFAULT NULL::character varying, p_file_size_label character varying DEFAULT NULL::character varying, p_file_extension character varying DEFAULT NULL::character varying) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.documents (
        document_id,
        player_id,
        club_id,
        document_name,
        document_type,
        document_date,
        file_size_label,
        file_data,
        file_extension,
        created_at
    )
    VALUES (
        p_document_id,
        p_player_id,
        p_club_id,
        p_document_name,
        p_document_type,
        p_document_date,
        p_file_size_label,
        p_file_data,
        p_file_extension,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_documents_insert(p_document_id character varying, p_document_name character varying, p_document_type character varying, p_document_date timestamp without time zone, p_created_at timestamp without time zone, p_file_data bytea, p_player_id character varying, p_club_id character varying, p_file_size_label character varying, p_file_extension character varying) OWNER TO postgres;

--
-- TOC entry 301 (class 1255 OID 19056)
-- Name: fn_documents_update(character varying, character varying, character varying, timestamp without time zone, bytea, character varying, character varying, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_documents_update(p_document_id character varying, p_document_name character varying, p_document_type character varying, p_document_date timestamp without time zone, p_file_data bytea DEFAULT NULL::bytea, p_player_id character varying DEFAULT NULL::character varying, p_club_id character varying DEFAULT NULL::character varying, p_file_size_label character varying DEFAULT NULL::character varying, p_file_extension character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.documents
    SET
        player_id       = p_player_id,
        club_id         = p_club_id,
        document_name   = p_document_name,
        document_type   = p_document_type,
        document_date   = p_document_date,
        file_size_label = p_file_size_label,
        file_data       = p_file_data,
        file_extension  = p_file_extension
    WHERE document_id = p_document_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_documents_update(p_document_id character varying, p_document_name character varying, p_document_type character varying, p_document_date timestamp without time zone, p_file_data bytea, p_player_id character varying, p_club_id character varying, p_file_size_label character varying, p_file_extension character varying) OWNER TO postgres;

--
-- TOC entry 278 (class 1255 OID 19063)
-- Name: fn_emails_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_emails_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.emails
    WHERE email_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_emails_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 357 (class 1255 OID 19060)
-- Name: fn_emails_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_emails_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.emails
    WHERE email_id = p_id;
$$;


ALTER FUNCTION stf.fn_emails_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 322 (class 1255 OID 19058)
-- Name: fn_emails_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_emails_get_all() RETURNS TABLE(email_id character varying, player_id character varying, club_id character varying, recipient_email character varying, subject character varying, body text, sent_by_scout_id character varying, sent_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        email_id,
        player_id,
        club_id,
        recipient_email,
        subject,
        body,
        sent_by_scout_id,
        sent_at
    FROM stf.emails
    ORDER BY sent_at DESC;
$$;


ALTER FUNCTION stf.fn_emails_get_all() OWNER TO postgres;

--
-- TOC entry 279 (class 1255 OID 19059)
-- Name: fn_emails_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_emails_get_by_id(p_id character varying) RETURNS TABLE(email_id character varying, player_id character varying, club_id character varying, recipient_email character varying, subject character varying, body text, sent_by_scout_id character varying, sent_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        email_id,
        player_id,
        club_id,
        recipient_email,
        subject,
        body,
        sent_by_scout_id,
        sent_at
    FROM stf.emails
    WHERE email_id = p_id;
$$;


ALTER FUNCTION stf.fn_emails_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 276 (class 1255 OID 19061)
-- Name: fn_emails_insert(character varying, character varying, character varying, text, character varying, timestamp without time zone, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_emails_insert(p_email_id character varying, p_recipient_email character varying, p_subject character varying, p_body text, p_sent_by_scout_id character varying, p_sent_at timestamp without time zone, p_player_id character varying DEFAULT NULL::character varying, p_club_id character varying DEFAULT NULL::character varying) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.emails (
        email_id,
        player_id,
        club_id,
        recipient_email,
        subject,
        body,
        sent_by_scout_id,
        sent_at
    )
    VALUES (
        p_email_id,
        p_player_id,
        p_club_id,
        p_recipient_email,
        p_subject,
        p_body,
        p_sent_by_scout_id,
        p_sent_at
    );
$$;


ALTER FUNCTION stf.fn_emails_insert(p_email_id character varying, p_recipient_email character varying, p_subject character varying, p_body text, p_sent_by_scout_id character varying, p_sent_at timestamp without time zone, p_player_id character varying, p_club_id character varying) OWNER TO postgres;

--
-- TOC entry 294 (class 1255 OID 19062)
-- Name: fn_emails_update(character varying, character varying, character varying, text, character varying, timestamp without time zone, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_emails_update(p_email_id character varying, p_recipient_email character varying, p_subject character varying, p_body text, p_sent_by_scout_id character varying, p_sent_at timestamp without time zone, p_player_id character varying DEFAULT NULL::character varying, p_club_id character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.emails
    SET
        player_id        = p_player_id,
        club_id          = p_club_id,
        recipient_email  = p_recipient_email,
        subject          = p_subject,
        body             = p_body,
        sent_by_scout_id = p_sent_by_scout_id,
        sent_at          = p_sent_at
    WHERE email_id = p_email_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_emails_update(p_email_id character varying, p_recipient_email character varying, p_subject character varying, p_body text, p_sent_by_scout_id character varying, p_sent_at timestamp without time zone, p_player_id character varying, p_club_id character varying) OWNER TO postgres;

--
-- TOC entry 369 (class 1255 OID 19072)
-- Name: fn_notes_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.notes
    WHERE note_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_notes_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 325 (class 1255 OID 19068)
-- Name: fn_notes_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.notes
    WHERE note_id = p_id;
$$;


ALTER FUNCTION stf.fn_notes_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 365 (class 1255 OID 19064)
-- Name: fn_notes_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_get_all() RETURNS TABLE(note_id character varying, player_id character varying, club_id character varying, topic character varying, description text, category character varying, follow_up_date date, created_by_scout_id character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        note_id,
        player_id,
        club_id,
        topic,
        description,
        category,
        follow_up_date,
        created_by_scout_id,
        created_at
    FROM stf.notes
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_notes_get_all() OWNER TO postgres;

--
-- TOC entry 260 (class 1255 OID 19067)
-- Name: fn_notes_get_by_club_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_get_by_club_id(p_club_id character varying) RETURNS TABLE(note_id character varying, player_id character varying, club_id character varying, topic character varying, description text, category character varying, follow_up_date date, created_by_scout_id character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        note_id,
        player_id,
        club_id,
        topic,
        description,
        category,
        follow_up_date,
        created_by_scout_id,
        created_at
    FROM stf.notes
    WHERE club_id = p_club_id
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_notes_get_by_club_id(p_club_id character varying) OWNER TO postgres;

--
-- TOC entry 304 (class 1255 OID 19065)
-- Name: fn_notes_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_get_by_id(p_id character varying) RETURNS TABLE(note_id character varying, player_id character varying, club_id character varying, topic character varying, description text, category character varying, follow_up_date date, created_by_scout_id character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        note_id,
        player_id,
        club_id,
        topic,
        description,
        category,
        follow_up_date,
        created_by_scout_id,
        created_at
    FROM stf.notes
    WHERE note_id = p_id;
$$;


ALTER FUNCTION stf.fn_notes_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 270 (class 1255 OID 19066)
-- Name: fn_notes_get_by_player_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_get_by_player_id(p_player_id character varying) RETURNS TABLE(note_id character varying, player_id character varying, club_id character varying, topic character varying, description text, category character varying, follow_up_date date, created_by_scout_id character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        note_id,
        player_id,
        club_id,
        topic,
        description,
        category,
        follow_up_date,
        created_by_scout_id,
        created_at
    FROM stf.notes
    WHERE player_id = p_player_id
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_notes_get_by_player_id(p_player_id character varying) OWNER TO postgres;

--
-- TOC entry 373 (class 1255 OID 19069)
-- Name: fn_notes_get_max_id(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_get_max_id() RETURNS character varying
    LANGUAGE sql STABLE
    AS $_$
    SELECT note_id
    FROM stf.notes
    WHERE note_id ~ '^n\d+$'
    ORDER BY CAST(SUBSTRING(note_id, 2) AS INTEGER) DESC
    LIMIT 1;
$_$;


ALTER FUNCTION stf.fn_notes_get_max_id() OWNER TO postgres;

--
-- TOC entry 289 (class 1255 OID 19070)
-- Name: fn_notes_insert(character varying, character varying, text, character varying, character varying, timestamp without time zone, character varying, character varying, date); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_insert(p_note_id character varying, p_topic character varying, p_description text, p_category character varying, p_created_by_scout_id character varying, p_created_at timestamp without time zone, p_player_id character varying DEFAULT NULL::character varying, p_club_id character varying DEFAULT NULL::character varying, p_follow_up_date date DEFAULT NULL::date) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.notes (
        note_id,
        player_id,
        club_id,
        topic,
        description,
        category,
        follow_up_date,
        created_by_scout_id,
        created_at
    )
    VALUES (
        p_note_id,
        p_player_id,
        p_club_id,
        p_topic,
        p_description,
        p_category,
        p_follow_up_date,
        p_created_by_scout_id,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_notes_insert(p_note_id character varying, p_topic character varying, p_description text, p_category character varying, p_created_by_scout_id character varying, p_created_at timestamp without time zone, p_player_id character varying, p_club_id character varying, p_follow_up_date date) OWNER TO postgres;

--
-- TOC entry 306 (class 1255 OID 19071)
-- Name: fn_notes_update(character varying, character varying, text, character varying, date); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_notes_update(p_note_id character varying, p_topic character varying, p_description text, p_category character varying, p_follow_up_date date DEFAULT NULL::date) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.notes
    SET
        topic          = p_topic,
        description    = p_description,
        category       = p_category,
        follow_up_date = p_follow_up_date
    WHERE note_id = p_note_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_notes_update(p_note_id character varying, p_topic character varying, p_description text, p_category character varying, p_follow_up_date date) OWNER TO postgres;

--
-- TOC entry 359 (class 1255 OID 19026)
-- Name: fn_review_ratings_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_ratings_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.review_ratings
    WHERE review_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_review_ratings_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 360 (class 1255 OID 19023)
-- Name: fn_review_ratings_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_ratings_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.review_ratings
    WHERE review_id = p_id;
$$;


ALTER FUNCTION stf.fn_review_ratings_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 354 (class 1255 OID 19021)
-- Name: fn_review_ratings_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_ratings_get_all() RETURNS TABLE(review_id character varying, passing numeric, shooting numeric, dribbling numeric, tactical_awareness numeric, defensive_contribution numeric, physical_strength numeric, behavior numeric, overall_performance numeric)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        passing,
        shooting,
        dribbling,
        tactical_awareness,
        defensive_contribution,
        physical_strength,
        behavior,
        overall_performance
    FROM stf.review_ratings
    ORDER BY review_id;
$$;


ALTER FUNCTION stf.fn_review_ratings_get_all() OWNER TO postgres;

--
-- TOC entry 258 (class 1255 OID 19022)
-- Name: fn_review_ratings_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_ratings_get_by_id(p_id character varying) RETURNS TABLE(review_id character varying, passing numeric, shooting numeric, dribbling numeric, tactical_awareness numeric, defensive_contribution numeric, physical_strength numeric, behavior numeric, overall_performance numeric)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        passing,
        shooting,
        dribbling,
        tactical_awareness,
        defensive_contribution,
        physical_strength,
        behavior,
        overall_performance
    FROM stf.review_ratings
    WHERE review_id = p_id;
$$;


ALTER FUNCTION stf.fn_review_ratings_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 356 (class 1255 OID 19024)
-- Name: fn_review_ratings_insert(character varying, numeric, numeric, numeric, numeric, numeric, numeric, numeric, numeric); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_ratings_insert(p_review_id character varying, p_passing numeric, p_shooting numeric, p_dribbling numeric, p_tactical_awareness numeric, p_defensive_contribution numeric, p_physical_strength numeric, p_behavior numeric, p_overall_performance numeric) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.review_ratings (
        review_id,
        passing,
        shooting,
        dribbling,
        tactical_awareness,
        defensive_contribution,
        physical_strength,
        behavior,
        overall_performance
    )
    VALUES (
        p_review_id,
        p_passing,
        p_shooting,
        p_dribbling,
        p_tactical_awareness,
        p_defensive_contribution,
        p_physical_strength,
        p_behavior,
        p_overall_performance
    );
$$;


ALTER FUNCTION stf.fn_review_ratings_insert(p_review_id character varying, p_passing numeric, p_shooting numeric, p_dribbling numeric, p_tactical_awareness numeric, p_defensive_contribution numeric, p_physical_strength numeric, p_behavior numeric, p_overall_performance numeric) OWNER TO postgres;

--
-- TOC entry 367 (class 1255 OID 19025)
-- Name: fn_review_ratings_update(character varying, numeric, numeric, numeric, numeric, numeric, numeric, numeric, numeric); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_ratings_update(p_review_id character varying, p_passing numeric, p_shooting numeric, p_dribbling numeric, p_tactical_awareness numeric, p_defensive_contribution numeric, p_physical_strength numeric, p_behavior numeric, p_overall_performance numeric) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.review_ratings
    SET
        passing                = p_passing,
        shooting               = p_shooting,
        dribbling              = p_dribbling,
        tactical_awareness     = p_tactical_awareness,
        defensive_contribution = p_defensive_contribution,
        physical_strength      = p_physical_strength,
        behavior               = p_behavior,
        overall_performance    = p_overall_performance
    WHERE review_id = p_review_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_review_ratings_update(p_review_id character varying, p_passing numeric, p_shooting numeric, p_dribbling numeric, p_tactical_awareness numeric, p_defensive_contribution numeric, p_physical_strength numeric, p_behavior numeric, p_overall_performance numeric) OWNER TO postgres;

--
-- TOC entry 277 (class 1255 OID 19051)
-- Name: fn_review_skill_details_delete(character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_skill_details_delete(p_review_id character varying, p_skill_key character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.review_skill_details
    WHERE review_id = p_review_id
      AND skill_key = p_skill_key;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_review_skill_details_delete(p_review_id character varying, p_skill_key character varying) OWNER TO postgres;

--
-- TOC entry 252 (class 1255 OID 19048)
-- Name: fn_review_skill_details_exists(character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_skill_details_exists(p_review_id character varying, p_skill_key character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.review_skill_details
    WHERE review_id = p_review_id
      AND skill_key = p_skill_key;
$$;


ALTER FUNCTION stf.fn_review_skill_details_exists(p_review_id character varying, p_skill_key character varying) OWNER TO postgres;

--
-- TOC entry 290 (class 1255 OID 19045)
-- Name: fn_review_skill_details_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_skill_details_get_all() RETURNS TABLE(review_id character varying, skill_key character varying, rating numeric, comment_text text, follow_up_date date)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        skill_key,
        rating,
        comment_text,
        follow_up_date
    FROM stf.review_skill_details
    ORDER BY review_id, skill_key;
$$;


ALTER FUNCTION stf.fn_review_skill_details_get_all() OWNER TO postgres;

--
-- TOC entry 272 (class 1255 OID 19046)
-- Name: fn_review_skill_details_get_by_id(character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_skill_details_get_by_id(p_review_id character varying, p_skill_key character varying) RETURNS TABLE(review_id character varying, skill_key character varying, rating numeric, comment_text text, follow_up_date date)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        skill_key,
        rating,
        comment_text,
        follow_up_date
    FROM stf.review_skill_details
    WHERE review_id = p_review_id
      AND skill_key = p_skill_key;
$$;


ALTER FUNCTION stf.fn_review_skill_details_get_by_id(p_review_id character varying, p_skill_key character varying) OWNER TO postgres;

--
-- TOC entry 280 (class 1255 OID 19047)
-- Name: fn_review_skill_details_get_by_review_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_skill_details_get_by_review_id(p_review_id character varying) RETURNS TABLE(review_id character varying, skill_key character varying, rating numeric, comment_text text, follow_up_date date)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        skill_key,
        rating,
        comment_text,
        follow_up_date
    FROM stf.review_skill_details
    WHERE review_id = p_review_id
    ORDER BY skill_key;
$$;


ALTER FUNCTION stf.fn_review_skill_details_get_by_review_id(p_review_id character varying) OWNER TO postgres;

--
-- TOC entry 316 (class 1255 OID 19049)
-- Name: fn_review_skill_details_insert(character varying, character varying, numeric, text, date); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_skill_details_insert(p_review_id character varying, p_skill_key character varying, p_rating numeric, p_comment_text text DEFAULT NULL::text, p_follow_up_date date DEFAULT NULL::date) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.review_skill_details (
        review_id,
        skill_key,
        rating,
        comment_text,
        follow_up_date
    )
    VALUES (
        p_review_id,
        p_skill_key,
        p_rating,
        p_comment_text,
        p_follow_up_date
    );
$$;


ALTER FUNCTION stf.fn_review_skill_details_insert(p_review_id character varying, p_skill_key character varying, p_rating numeric, p_comment_text text, p_follow_up_date date) OWNER TO postgres;

--
-- TOC entry 339 (class 1255 OID 19050)
-- Name: fn_review_skill_details_update(character varying, character varying, numeric, text, date); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_review_skill_details_update(p_review_id character varying, p_skill_key character varying, p_rating numeric, p_comment_text text DEFAULT NULL::text, p_follow_up_date date DEFAULT NULL::date) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.review_skill_details
    SET
        rating           = p_rating,
        comment_text     = p_comment_text,
        follow_up_date   = p_follow_up_date
    WHERE review_id = p_review_id
      AND skill_key = p_skill_key;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_review_skill_details_update(p_review_id character varying, p_skill_key character varying, p_rating numeric, p_comment_text text, p_follow_up_date date) OWNER TO postgres;

--
-- TOC entry 283 (class 1255 OID 18999)
-- Name: fn_reviews_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.reviews
    WHERE review_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_reviews_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 249 (class 1255 OID 18996)
-- Name: fn_reviews_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.reviews
    WHERE review_id = p_id;
$$;


ALTER FUNCTION stf.fn_reviews_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 362 (class 1255 OID 18992)
-- Name: fn_reviews_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_get_all() RETURNS TABLE(review_id character varying, player_id character varying, scout_id character varying, match_date date, club1_id character varying, club2_id character varying, notes text, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        player_id,
        scout_id,
        match_date,
        club1_id,
        club2_id,
        notes,
        created_at
    FROM stf.reviews
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_reviews_get_all() OWNER TO postgres;

--
-- TOC entry 315 (class 1255 OID 18993)
-- Name: fn_reviews_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_get_by_id(p_id character varying) RETURNS TABLE(review_id character varying, player_id character varying, scout_id character varying, match_date date, club1_id character varying, club2_id character varying, notes text, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        player_id,
        scout_id,
        match_date,
        club1_id,
        club2_id,
        notes,
        created_at
    FROM stf.reviews
    WHERE review_id = p_id;
$$;


ALTER FUNCTION stf.fn_reviews_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 286 (class 1255 OID 18994)
-- Name: fn_reviews_get_by_player_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_get_by_player_id(p_player_id character varying) RETURNS TABLE(review_id character varying, player_id character varying, scout_id character varying, match_date date, club1_id character varying, club2_id character varying, notes text, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        player_id,
        scout_id,
        match_date,
        club1_id,
        club2_id,
        notes,
        created_at
    FROM stf.reviews
    WHERE player_id = p_player_id
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_reviews_get_by_player_id(p_player_id character varying) OWNER TO postgres;

--
-- TOC entry 318 (class 1255 OID 18995)
-- Name: fn_reviews_get_by_scout_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_get_by_scout_id(p_scout_id character varying) RETURNS TABLE(review_id character varying, player_id character varying, scout_id character varying, match_date date, club1_id character varying, club2_id character varying, notes text, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        review_id,
        player_id,
        scout_id,
        match_date,
        club1_id,
        club2_id,
        notes,
        created_at
    FROM stf.reviews
    WHERE scout_id = p_scout_id
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_reviews_get_by_scout_id(p_scout_id character varying) OWNER TO postgres;

--
-- TOC entry 323 (class 1255 OID 18997)
-- Name: fn_reviews_insert(character varying, character varying, character varying, date, timestamp without time zone, character varying, character varying, text); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_insert(p_review_id character varying, p_player_id character varying, p_scout_id character varying, p_match_date date DEFAULT NULL::date, p_created_at timestamp without time zone, p_club1_id character varying DEFAULT NULL::character varying, p_club2_id character varying DEFAULT NULL::character varying, p_notes text DEFAULT NULL::text) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.reviews (
        review_id,
        player_id,
        scout_id,
        match_date,
        club1_id,
        club2_id,
        notes,
        created_at
    )
    VALUES (
        p_review_id,
        p_player_id,
        p_scout_id,
        p_match_date,
        p_club1_id,
        p_club2_id,
        p_notes,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_reviews_insert(p_review_id character varying, p_player_id character varying, p_scout_id character varying, p_match_date date, p_created_at timestamp without time zone, p_club1_id character varying, p_club2_id character varying, p_notes text) OWNER TO postgres;

--
-- TOC entry 299 (class 1255 OID 18998)
-- Name: fn_reviews_update(character varying, date, character varying, character varying, text); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_reviews_update(p_review_id character varying, p_match_date date DEFAULT NULL::date, p_club1_id character varying DEFAULT NULL::character varying, p_club2_id character varying DEFAULT NULL::character varying, p_notes text DEFAULT NULL::text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.reviews
    SET
        match_date = p_match_date,
        club1_id   = p_club1_id,
        club2_id   = p_club2_id,
        notes      = p_notes
    WHERE review_id = p_review_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_reviews_update(p_review_id character varying, p_match_date date, p_club1_id character varying, p_club2_id character varying, p_notes text) OWNER TO postgres;

--
-- TOC entry 334 (class 1255 OID 18957)
-- Name: fn_scouts_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_scouts_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.scouts
    WHERE scout_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_scouts_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 261 (class 1255 OID 18953)
-- Name: fn_scouts_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_scouts_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.scouts
    WHERE scout_id = p_id;
$$;


ALTER FUNCTION stf.fn_scouts_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 273 (class 1255 OID 18951)
-- Name: fn_scouts_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_scouts_get_all() RETURNS TABLE(scout_id character varying, scout_name character varying, role_name character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        scout_id,
        scout_name,
        role_name,
        created_at
    FROM stf.scouts
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_scouts_get_all() OWNER TO postgres;

--
-- TOC entry 364 (class 1255 OID 18952)
-- Name: fn_scouts_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_scouts_get_by_id(p_id character varying) RETURNS TABLE(scout_id character varying, scout_name character varying, role_name character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        scout_id,
        scout_name,
        role_name,
        created_at
    FROM stf.scouts
    WHERE scout_id = p_id;
$$;


ALTER FUNCTION stf.fn_scouts_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 308 (class 1255 OID 18955)
-- Name: fn_scouts_insert(character varying, character varying, character varying, timestamp without time zone); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_scouts_insert(p_scout_id character varying, p_scout_name character varying, p_role_name character varying, p_created_at timestamp without time zone) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.scouts (
        scout_id,
        scout_name,
        role_name,
        created_at
    )
    VALUES (
        p_scout_id,
        p_scout_name,
        p_role_name,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_scouts_insert(p_scout_id character varying, p_scout_name character varying, p_role_name character varying, p_created_at timestamp without time zone) OWNER TO postgres;

--
-- TOC entry 327 (class 1255 OID 18954)
-- Name: fn_scouts_name_exists(character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_scouts_name_exists(p_scout_name character varying, p_exclude_scout_id character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.scouts
    WHERE scout_name = p_scout_name
      AND (p_exclude_scout_id IS NULL OR scout_id <> p_exclude_scout_id);
$$;


ALTER FUNCTION stf.fn_scouts_name_exists(p_scout_name character varying, p_exclude_scout_id character varying) OWNER TO postgres;

--
-- TOC entry 377 (class 1255 OID 18956)
-- Name: fn_scouts_update(character varying, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_scouts_update(p_scout_id character varying, p_scout_name character varying, p_role_name character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.scouts
    SET
        scout_name = p_scout_name,
        role_name  = p_role_name
    WHERE scout_id = p_scout_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_scouts_update(p_scout_id character varying, p_scout_name character varying, p_role_name character varying) OWNER TO postgres;

--
-- TOC entry 340 (class 1255 OID 18950)
-- Name: fn_tasks_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_tasks_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.tasks
    WHERE task_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_tasks_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 375 (class 1255 OID 18947)
-- Name: fn_tasks_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_tasks_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.tasks
    WHERE task_id = p_id;
$$;


ALTER FUNCTION stf.fn_tasks_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 298 (class 1255 OID 18945)
-- Name: fn_tasks_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_tasks_get_all() RETURNS TABLE(task_id character varying, title character varying, description text, player_id character varying, club_id character varying, assigned_to_scout_id character varying, due_date date, status character varying, source character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        task_id,
        title,
        description,
        player_id,
        club_id,
        assigned_to_scout_id,
        due_date,
        status,
        source,
        created_at
    FROM stf.tasks
    ORDER BY due_date DESC, created_at DESC;
$$;


ALTER FUNCTION stf.fn_tasks_get_all() OWNER TO postgres;

--
-- TOC entry 346 (class 1255 OID 18946)
-- Name: fn_tasks_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_tasks_get_by_id(p_id character varying) RETURNS TABLE(task_id character varying, title character varying, description text, player_id character varying, club_id character varying, assigned_to_scout_id character varying, due_date date, status character varying, source character varying, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        task_id,
        title,
        description,
        player_id,
        club_id,
        assigned_to_scout_id,
        due_date,
        status,
        source,
        created_at
    FROM stf.tasks
    WHERE task_id = p_id;
$$;


ALTER FUNCTION stf.fn_tasks_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 311 (class 1255 OID 18948)
-- Name: fn_tasks_insert(character varying, character varying, character varying, date, character varying, character varying, timestamp without time zone, text, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_tasks_insert(p_task_id character varying, p_title character varying, p_assigned_to_scout_id character varying, p_due_date date, p_status character varying, p_source character varying, p_created_at timestamp without time zone, p_description text DEFAULT NULL::text, p_player_id character varying DEFAULT NULL::character varying, p_club_id character varying DEFAULT NULL::character varying) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.tasks (
        task_id,
        title,
        description,
        player_id,
        club_id,
        assigned_to_scout_id,
        due_date,
        status,
        source,
        created_at
    )
    VALUES (
        p_task_id,
        p_title,
        p_description,
        p_player_id,
        p_club_id,
        p_assigned_to_scout_id,
        p_due_date,
        p_status,
        p_source,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_tasks_insert(p_task_id character varying, p_title character varying, p_assigned_to_scout_id character varying, p_due_date date, p_status character varying, p_source character varying, p_created_at timestamp without time zone, p_description text, p_player_id character varying, p_club_id character varying) OWNER TO postgres;

--
-- TOC entry 300 (class 1255 OID 18949)
-- Name: fn_tasks_update(character varying, character varying, character varying, date, character varying, character varying, text, character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_tasks_update(p_task_id character varying, p_title character varying, p_assigned_to_scout_id character varying, p_due_date date, p_status character varying, p_source character varying, p_description text DEFAULT NULL::text, p_player_id character varying DEFAULT NULL::character varying, p_club_id character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.tasks
    SET
        title                = p_title,
        description          = p_description,
        player_id            = p_player_id,
        club_id              = p_club_id,
        assigned_to_scout_id = p_assigned_to_scout_id,
        due_date             = p_due_date,
        status               = p_status,
        source               = p_source
    WHERE task_id = p_task_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_tasks_update(p_task_id character varying, p_title character varying, p_assigned_to_scout_id character varying, p_due_date date, p_status character varying, p_source character varying, p_description text, p_player_id character varying, p_club_id character varying) OWNER TO postgres;

--
-- TOC entry 352 (class 1255 OID 18944)
-- Name: fn_templates_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_templates_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.templates
    WHERE template_id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_templates_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 328 (class 1255 OID 18940)
-- Name: fn_templates_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_templates_exists(p_id character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.templates
    WHERE template_id = p_id;
$$;


ALTER FUNCTION stf.fn_templates_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 281 (class 1255 OID 18938)
-- Name: fn_templates_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_templates_get_all() RETURNS TABLE(template_id character varying, template_name character varying, template_type character varying, subject character varying, body text, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        template_id,
        template_name,
        template_type,
        subject,
        body,
        created_at
    FROM stf.templates
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_templates_get_all() OWNER TO postgres;

--
-- TOC entry 302 (class 1255 OID 18939)
-- Name: fn_templates_get_by_id(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_templates_get_by_id(p_id character varying) RETURNS TABLE(template_id character varying, template_name character varying, template_type character varying, subject character varying, body text, created_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        template_id,
        template_name,
        template_type,
        subject,
        body,
        created_at
    FROM stf.templates
    WHERE template_id = p_id;
$$;


ALTER FUNCTION stf.fn_templates_get_by_id(p_id character varying) OWNER TO postgres;

--
-- TOC entry 335 (class 1255 OID 18942)
-- Name: fn_templates_insert(character varying, character varying, character varying, text, timestamp without time zone, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_templates_insert(p_template_id character varying, p_template_name character varying, p_template_type character varying, p_body text, p_created_at timestamp without time zone, p_subject character varying DEFAULT NULL::character varying) RETURNS void
    LANGUAGE sql
    AS $$
    INSERT INTO stf.templates (
        template_id,
        template_name,
        template_type,
        subject,
        body,
        created_at
    )
    VALUES (
        p_template_id,
        p_template_name,
        p_template_type,
        p_subject,
        p_body,
        p_created_at
    );
$$;


ALTER FUNCTION stf.fn_templates_insert(p_template_id character varying, p_template_name character varying, p_template_type character varying, p_body text, p_created_at timestamp without time zone, p_subject character varying) OWNER TO postgres;

--
-- TOC entry 336 (class 1255 OID 18941)
-- Name: fn_templates_name_exists(character varying, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_templates_name_exists(p_template_name character varying, p_exclude_template_id character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.templates
    WHERE template_name = p_template_name
      AND (p_exclude_template_id IS NULL OR template_id <> p_exclude_template_id);
$$;


ALTER FUNCTION stf.fn_templates_name_exists(p_template_name character varying, p_exclude_template_id character varying) OWNER TO postgres;

--
-- TOC entry 351 (class 1255 OID 18943)
-- Name: fn_templates_update(character varying, character varying, character varying, text, character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_templates_update(p_template_id character varying, p_template_name character varying, p_template_type character varying, p_body text, p_subject character varying DEFAULT NULL::character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.templates
    SET
        template_name = p_template_name,
        template_type = p_template_type,
        subject       = p_subject,
        body          = p_body
    WHERE template_id = p_template_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_templates_update(p_template_id character varying, p_template_name character varying, p_template_type character varying, p_body text, p_subject character varying) OWNER TO postgres;

--
-- TOC entry 332 (class 1255 OID 18937)
-- Name: fn_users_delete(bigint); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_delete(p_id bigint) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    DELETE FROM stf.users
    WHERE id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_users_delete(p_id bigint) OWNER TO postgres;

--
-- TOC entry 347 (class 1255 OID 18934)
-- Name: fn_users_email_exists(character varying, bigint); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_email_exists(p_email character varying, p_exclude_user_id bigint DEFAULT NULL::bigint) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.users
    WHERE email = p_email
      AND (p_exclude_user_id IS NULL OR id <> p_exclude_user_id);
$$;


ALTER FUNCTION stf.fn_users_email_exists(p_email character varying, p_exclude_user_id bigint) OWNER TO postgres;

--
-- TOC entry 250 (class 1255 OID 18933)
-- Name: fn_users_exists(bigint); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_exists(p_id bigint) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
    SELECT COUNT(1)::INTEGER
    FROM stf.users
    WHERE id = p_id;
$$;


ALTER FUNCTION stf.fn_users_exists(p_id bigint) OWNER TO postgres;

--
-- TOC entry 363 (class 1255 OID 18930)
-- Name: fn_users_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_get_all() RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        id,
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at,
        updated_at
    FROM stf.users
    ORDER BY created_at DESC;
$$;


ALTER FUNCTION stf.fn_users_get_all() OWNER TO postgres;

--
-- TOC entry 275 (class 1255 OID 18932)
-- Name: fn_users_get_by_email(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_get_by_email(p_email character varying) RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        id,
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at,
        updated_at
    FROM stf.users
    WHERE email = p_email;
$$;


ALTER FUNCTION stf.fn_users_get_by_email(p_email character varying) OWNER TO postgres;

--
-- TOC entry 266 (class 1255 OID 18931)
-- Name: fn_users_get_by_id(bigint); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_get_by_id(p_id bigint) RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE sql STABLE
    AS $$
    SELECT
        id,
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at,
        updated_at
    FROM stf.users
    WHERE id = p_id;
$$;


ALTER FUNCTION stf.fn_users_get_by_id(p_id bigint) OWNER TO postgres;

--
-- TOC entry 326 (class 1255 OID 18935)
-- Name: fn_users_insert(character varying, character varying, character varying, character varying, timestamp without time zone, character varying, boolean); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_created_at timestamp without time zone, p_phone character varying DEFAULT NULL::character varying, p_status boolean DEFAULT true) RETURNS bigint
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_new_id BIGINT;
BEGIN
    INSERT INTO stf.users (
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at
    )
    VALUES (
        p_name,
        p_email,
        p_password,
        p_role,
        p_phone,
        p_status,
        p_created_at
    )
    RETURNING id INTO v_new_id;

    RETURN v_new_id;
END;
$$;


ALTER FUNCTION stf.fn_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_created_at timestamp without time zone, p_phone character varying, p_status boolean) OWNER TO postgres;

--
-- TOC entry 333 (class 1255 OID 18936)
-- Name: fn_users_update(bigint, character varying, character varying, character varying, timestamp without time zone, character varying, boolean); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.fn_users_update(p_id bigint, p_name character varying, p_email character varying, p_role character varying, p_updated_at timestamp without time zone, p_phone character varying DEFAULT NULL::character varying, p_status boolean DEFAULT NULL::boolean) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_affected INTEGER;
BEGIN
    UPDATE stf.users
    SET
        name       = p_name,
        email      = p_email,
        role       = p_role,
        phone      = p_phone,
        status     = p_status,
        updated_at = p_updated_at
    WHERE id = p_id;

    GET DIAGNOSTICS v_affected = ROW_COUNT;
    RETURN v_affected;
END;
$$;


ALTER FUNCTION stf.fn_users_update(p_id bigint, p_name character varying, p_email character varying, p_role character varying, p_updated_at timestamp without time zone, p_phone character varying, p_status boolean) OWNER TO postgres;

--
-- TOC entry 314 (class 1255 OID 18880)
-- Name: sp_players_delete(bigint); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_delete(p_id bigint) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_count INT;
BEGIN

DELETE FROM stf.players
WHERE player_id = p_id::TEXT;

GET DIAGNOSTICS v_count = ROW_COUNT;

RETURN v_count;

END;
$$;


ALTER FUNCTION stf.sp_players_delete(p_id bigint) OWNER TO postgres;

--
-- TOC entry 374 (class 1255 OID 18879)
-- Name: sp_players_delete(text); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_delete(p_player_id text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_count INT;
BEGIN

DELETE FROM stf.players
WHERE player_id = p_player_id;

GET DIAGNOSTICS v_count = ROW_COUNT;

RETURN v_count;

END;
$$;


ALTER FUNCTION stf.sp_players_delete(p_player_id text) OWNER TO postgres;

--
-- TOC entry 371 (class 1255 OID 18826)
-- Name: sp_players_delete(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_delete(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE v_count INT;
BEGIN

DELETE FROM stf.players
WHERE player_id = p_id;

GET DIAGNOSTICS v_count = ROW_COUNT;

RETURN v_count;

END;
$$;


ALTER FUNCTION stf.sp_players_delete(p_id character varying) OWNER TO postgres;

--
-- TOC entry 370 (class 1255 OID 18827)
-- Name: sp_players_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_exists(p_id character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE v_count INT;
BEGIN

SELECT COUNT(*)
INTO v_count
FROM stf.players
WHERE player_id = p_id;

RETURN v_count;

END;
$$;


ALTER FUNCTION stf.sp_players_exists(p_id character varying) OWNER TO postgres;

--
-- TOC entry 355 (class 1255 OID 18877)
-- Name: sp_players_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_get_all() RETURNS TABLE(player_id character varying, full_name character varying, date_of_birth date, nationality character varying, position_code character varying, preferred_foot character varying, height_cm integer, weight_kg integer, current_club_id character varying, contract_start_date date, contract_end_date date, agent_name character varying, agent_scout_id character varying, contact_info character varying, profile_image_url character varying, player_email character varying, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN

RETURN QUERY
SELECT
    p.player_id,
    p.full_name,
    p.date_of_birth,
    p.nationality,
    p.position_code,
    p.preferred_foot,
    p.height_cm,
    p.weight_kg,
    p.current_club_id,
    p.contract_start_date,
    p.contract_end_date,
    p.agent_name,
    p.agent_scout_id,
    p.contact_info,
    p.profile_image_url,
    p.player_email,
    p.created_at,
    p.updated_at
FROM stf.players p;

END;
$$;


ALTER FUNCTION stf.sp_players_get_all() OWNER TO postgres;

--
-- TOC entry 265 (class 1255 OID 18875)
-- Name: sp_players_get_by_id(bigint); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_get_by_id(p_id bigint) RETURNS TABLE(player_id character varying, full_name character varying, date_of_birth date, nationality character varying, position_code character varying, preferred_foot character varying, height_cm integer, weight_kg integer, current_club_id character varying, contract_start_date date, contract_end_date date, agent_name character varying, agent_scout_id character varying, contact_info character varying, profile_image_url character varying, player_email character varying, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN

RETURN QUERY
SELECT
    p.player_id,
    p.full_name,
    p.date_of_birth,
    p.nationality,
    p.position_code,
    p.preferred_foot,
    p.height_cm,
    p.weight_kg,
    p.current_club_id,
    p.contract_start_date,
    p.contract_end_date,
    p.agent_name,
    p.agent_scout_id,
    p.contact_info,
    p.profile_image_url,
    p.player_email,
    p.created_at,
    p.updated_at
FROM stf.players p
WHERE p.player_id::BIGINT = p_id;

END;
$$;


ALTER FUNCTION stf.sp_players_get_by_id(p_id bigint) OWNER TO postgres;

--
-- TOC entry 309 (class 1255 OID 18870)
-- Name: sp_players_insert(text, text, date, text, text, text, integer, integer, text, date, date, text, text, text, text, timestamp with time zone, timestamp with time zone, text); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_insert(p_player_id text, p_full_name text, p_date_of_birth date, p_nationality text, p_position_code text, p_preferred_foot text, p_height_cm integer, p_weight_kg integer, p_current_club_id text, p_contract_start_date date, p_contract_end_date date, p_agent_name text, p_agent_scout_id text, p_contact_info text, p_profile_image_url text, p_created_at timestamp with time zone, p_updated_at timestamp with time zone, p_player_email text) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN

INSERT INTO stf.players(
    player_id,
    full_name,
    date_of_birth,
    nationality,
    position_code,
    preferred_foot,
    height_cm,
    weight_kg,
    current_club_id,
    contract_start_date,
    contract_end_date,
    agent_name,
    agent_scout_id,
    contact_info,
    profile_image_url,
    created_at,
    updated_at,
    player_email,
)
VALUES (
    p_player_id,
    p_full_name,
    p_date_of_birth,
    p_nationality,
    p_position_code,
    p_preferred_foot,
    p_height_cm,
    p_weight_kg,
    p_current_club_id,
    p_contract_start_date,
    p_contract_end_date,
    p_agent_name,
    p_agent_scout_id,
    p_contact_info,
    p_profile_image_url, 
    p_created_at,
    p_updated_at,
    p_player_email,
);

END;
$$;


ALTER FUNCTION stf.sp_players_insert(p_player_id text, p_full_name text, p_date_of_birth date, p_nationality text, p_position_code text, p_preferred_foot text, p_height_cm integer, p_weight_kg integer, p_current_club_id text, p_contract_start_date date, p_contract_end_date date, p_agent_name text, p_agent_scout_id text, p_contact_info text, p_profile_image_url text, p_created_at timestamp with time zone, p_updated_at timestamp with time zone, p_player_email text) OWNER TO postgres;

--
-- TOC entry 292 (class 1255 OID 18876)
-- Name: sp_players_update(text, text, date, text, text, text, integer, integer, text, date, date, text, text, text, text, timestamp with time zone); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_players_update(p_player_id text, p_full_name text, p_date_of_birth date, p_nationality text, p_position_code text, p_preferred_foot text, p_height_cm integer, p_weight_kg integer, p_current_club_id text, p_contract_start_date date, p_contract_end_date date, p_agent_name text, p_agent_scout_id text, p_contact_info text, p_profile_image_url text, p_updated_at timestamp with time zone) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN

UPDATE stf.players
SET
    full_name = p_full_name,
    date_of_birth = p_date_of_birth,
    nationality = p_nationality,
    position_code = p_position_code,
    preferred_foot = p_preferred_foot,
    height_cm = p_height_cm,
    weight_kg = p_weight_kg,
    current_club_id = p_current_club_id,
    contract_start_date = p_contract_start_date,
    contract_end_date = p_contract_end_date,
    agent_name = p_agent_name,
    agent_scout_id = p_agent_scout_id,
    contact_info = p_contact_info,
    profile_image_url = p_profile_image_url,
    updated_at = p_updated_at
WHERE player_id = p_player_id;

END;
$$;


ALTER FUNCTION stf.sp_players_update(p_player_id text, p_full_name text, p_date_of_birth date, p_nationality text, p_position_code text, p_preferred_foot text, p_height_cm integer, p_weight_kg integer, p_current_club_id text, p_contract_start_date date, p_contract_end_date date, p_agent_name text, p_agent_scout_id text, p_contact_info text, p_profile_image_url text, p_updated_at timestamp with time zone) OWNER TO postgres;

--
-- TOC entry 338 (class 1255 OID 18820)
-- Name: sp_users_delete(bigint); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_users_delete(p_id bigint) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN

    DELETE FROM stf.users
    WHERE id = p_id;

END;
$$;


ALTER FUNCTION stf.sp_users_delete(p_id bigint) OWNER TO postgres;

--
-- TOC entry 310 (class 1255 OID 18821)
-- Name: sp_users_email_exists(character varying); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_users_email_exists(p_email character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE v_exists BOOLEAN;
BEGIN

    SELECT EXISTS (
        SELECT 1
        FROM stf.users
        WHERE email = p_email
    )
    INTO v_exists;

    RETURN v_exists;

END;
$$;


ALTER FUNCTION stf.sp_users_email_exists(p_email character varying) OWNER TO postgres;

--
-- TOC entry 312 (class 1255 OID 18817)
-- Name: sp_users_get_all(); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_users_get_all() RETURNS TABLE(id bigint, name character varying, email character varying, password character varying, role character varying, phone character varying, status boolean, created_at timestamp without time zone, updated_at timestamp without time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        u.id,
        u.name,
        u.email,
        u.password,
        u.role,
        u.phone,
        u.status,
        u.created_at,
        u.updated_at
    FROM stf.users u
    ORDER BY u.created_at DESC;
END;
$$;


ALTER FUNCTION stf.sp_users_get_all() OWNER TO postgres;

--
-- TOC entry 330 (class 1255 OID 18818)
-- Name: sp_users_insert(character varying, character varying, character varying, character varying, character varying, boolean); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_phone character varying, p_status boolean) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN

    INSERT INTO stf.users (
        name,
        email,
        password,
        role,
        phone,
        status,
        created_at,
        updated_at
    )
    VALUES (
        p_name,
        p_email,
        p_password,
        p_role,
        p_phone,
        p_status,
        CURRENT_TIMESTAMP,
        CURRENT_TIMESTAMP
    );

END;
$$;


ALTER FUNCTION stf.sp_users_insert(p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_phone character varying, p_status boolean) OWNER TO postgres;

--
-- TOC entry 324 (class 1255 OID 18819)
-- Name: sp_users_update(bigint, character varying, character varying, character varying, character varying, character varying, boolean); Type: FUNCTION; Schema: stf; Owner: postgres
--

CREATE FUNCTION stf.sp_users_update(p_id bigint, p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_phone character varying, p_status boolean) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN

    UPDATE stf.users
    SET
        name = p_name,
        email = p_email,
        password = p_password,
        role = p_role,
        phone = p_phone,
        status = p_status,
        updated_at = CURRENT_TIMESTAMP
    WHERE id = p_id;

END;
$$;


ALTER FUNCTION stf.sp_users_update(p_id bigint, p_name character varying, p_email character varying, p_password character varying, p_role character varying, p_phone character varying, p_status boolean) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 243 (class 1259 OID 19197)
-- Name: AspNetRoleClaims; Type: TABLE; Schema: auth; Owner: postgres
--

CREATE TABLE auth."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE auth."AspNetRoleClaims" OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 19196)
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: auth; Owner: postgres
--

ALTER TABLE auth."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME auth."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 240 (class 1259 OID 19170)
-- Name: AspNetRoles; Type: TABLE; Schema: auth; Owner: postgres
--

CREATE TABLE auth."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE auth."AspNetRoles" OWNER TO postgres;

--
-- TOC entry 245 (class 1259 OID 19212)
-- Name: AspNetUserClaims; Type: TABLE; Schema: auth; Owner: postgres
--

CREATE TABLE auth."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE auth."AspNetUserClaims" OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 19211)
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: auth; Owner: postgres
--

ALTER TABLE auth."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME auth."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 246 (class 1259 OID 19226)
-- Name: AspNetUserLogins; Type: TABLE; Schema: auth; Owner: postgres
--

CREATE TABLE auth."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE auth."AspNetUserLogins" OWNER TO postgres;

--
-- TOC entry 247 (class 1259 OID 19241)
-- Name: AspNetUserRoles; Type: TABLE; Schema: auth; Owner: postgres
--

CREATE TABLE auth."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE auth."AspNetUserRoles" OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 19260)
-- Name: AspNetUserTokens; Type: TABLE; Schema: auth; Owner: postgres
--

CREATE TABLE auth."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text
);


ALTER TABLE auth."AspNetUserTokens" OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 19178)
-- Name: AspNetUsers; Type: TABLE; Schema: auth; Owner: postgres
--

CREATE TABLE auth."AspNetUsers" (
    "Id" text NOT NULL,
    "FullName" text NOT NULL,
    "Role" text NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "InviteToken" text,
    "InviteTokenExpiry" timestamp with time zone,
    "IsInviteAccepted" boolean NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE auth."AspNetUsers" OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 19162)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 18169)
-- Name: players; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.players (
    id bigint NOT NULL,
    full_name character varying(200) NOT NULL,
    date_of_birth date,
    nationality character varying(100),
    player_position character varying(50),
    preferred_foot character varying(10),
    height_cm integer,
    weight_kg integer,
    current_club character varying(150),
    contract_start date,
    contract_end date,
    contract_status character varying(20),
    agent_name character varying(150),
    agent_contact character varying(150),
    created_at timestamp without time zone DEFAULT now(),
    updated_at timestamp without time zone DEFAULT now()
);


ALTER TABLE public.players OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 18168)
-- Name: players_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.players_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.players_id_seq OWNER TO postgres;

--
-- TOC entry 5216 (class 0 OID 0)
-- Dependencies: 221
-- Name: players_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.players_id_seq OWNED BY public.players.id;


--
-- TOC entry 224 (class 1259 OID 18182)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id bigint NOT NULL,
    name character varying(150) CONSTRAINT users_user_name_not_null NOT NULL,
    email character varying(200) NOT NULL,
    password character varying(255) CONSTRAINT users_user_password_not_null NOT NULL,
    role character varying(20) CONSTRAINT users_user_role_not_null NOT NULL,
    phone character varying(20),
    status boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT now(),
    updated_at timestamp without time zone DEFAULT now(),
    CONSTRAINT ck_users_role CHECK (((role)::text = ANY ((ARRAY['coach'::character varying, 'scout'::character varying, 'admin'::character varying])::text[])))
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 18181)
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_id_seq OWNER TO postgres;

--
-- TOC entry 5217 (class 0 OID 0)
-- Dependencies: 223
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- TOC entry 229 (class 1259 OID 18684)
-- Name: club_contacts; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.club_contacts (
    club_contact_id character varying(50) NOT NULL,
    club_id character varying(50) NOT NULL,
    contact_name character varying(150) NOT NULL,
    role_name character varying(100) NOT NULL,
    email character varying(254),
    phone character varying(50),
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE stf.club_contacts OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 18661)
-- Name: clubs; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.clubs (
    club_id character varying(50) NOT NULL,
    club_name character varying(150) NOT NULL,
    country character varying(100) NOT NULL,
    address_line character varying(300),
    logo_url character varying(500),
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE stf.clubs OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 18701)
-- Name: documents; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.documents (
    document_id character varying(50) NOT NULL,
    player_id character varying(50),
    club_id character varying(50),
    document_name character varying(255) NOT NULL,
    document_type character varying(50) NOT NULL,
    document_date timestamp without time zone NOT NULL,
    file_size_label character varying(50),
    file_data bytea,
    file_extension character varying(10),
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE stf.documents OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 18726)
-- Name: emails; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.emails (
    email_id character varying(50) NOT NULL,
    player_id character varying(50),
    club_id character varying(50),
    recipient_email character varying(254) NOT NULL,
    subject character varying(300) NOT NULL,
    body text NOT NULL,
    sent_by_scout_id character varying(50) NOT NULL,
    sent_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE stf.emails OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 18713)
-- Name: notes; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.notes (
    note_id character varying(50) NOT NULL,
    player_id character varying(50),
    club_id character varying(50),
    topic character varying(200) NOT NULL,
    description text NOT NULL,
    category character varying(30) NOT NULL,
    follow_up_date date,
    created_by_scout_id character varying(50) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE stf.notes OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 18858)
-- Name: players; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.players (
    player_id character varying(50) NOT NULL,
    full_name character varying(150) NOT NULL,
    date_of_birth date NOT NULL,
    nationality character varying(100),
    position_code character varying(10),
    preferred_foot character varying(10),
    height_cm integer,
    weight_kg integer,
    current_club_id character varying(50),
    contract_start_date date,
    contract_end_date date,
    agent_name character varying(150),
    agent_scout_id character varying(50),
    contact_info character varying(255),
    profile_image_url character varying(500),
    created_at timestamp without time zone,
    updated_at timestamp without time zone
);


ALTER TABLE stf.players OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 19001)
-- Name: review_ratings; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.review_ratings (
    review_id character varying(50) NOT NULL,
    passing numeric(3,1) NOT NULL,
    shooting numeric(3,1) NOT NULL,
    dribbling numeric(3,1) NOT NULL,
    tactical_awareness numeric(3,1) NOT NULL,
    defensive_contribution numeric(3,1) NOT NULL,
    physical_strength numeric(3,1) NOT NULL,
    behavior numeric(3,1) NOT NULL,
    overall_performance numeric(3,1) NOT NULL,
    CONSTRAINT ck_review_ratings_range CHECK ((((passing >= (0)::numeric) AND (passing <= (5)::numeric)) AND ((shooting >= (0)::numeric) AND (shooting <= (5)::numeric)) AND ((dribbling >= (0)::numeric) AND (dribbling <= (5)::numeric)) AND ((tactical_awareness >= (0)::numeric) AND (tactical_awareness <= (5)::numeric)) AND ((defensive_contribution >= (0)::numeric) AND (defensive_contribution <= (5)::numeric)) AND ((physical_strength >= (0)::numeric) AND (physical_strength <= (5)::numeric)) AND ((behavior >= (0)::numeric) AND (behavior <= (5)::numeric)) AND ((overall_performance >= (0)::numeric) AND (overall_performance <= (5)::numeric))))
);


ALTER TABLE stf.review_ratings OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 19028)
-- Name: review_skill_details; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.review_skill_details (
    review_id character varying(50) NOT NULL,
    skill_key character varying(50) NOT NULL,
    rating numeric(3,1) NOT NULL,
    comment_text text,
    follow_up_date date,
    CONSTRAINT ck_review_skill_details_rating CHECK (((rating >= (0)::numeric) AND (rating <= (5)::numeric))),
    CONSTRAINT ck_review_skill_details_skill CHECK (((skill_key)::text = ANY ((ARRAY['passing'::character varying, 'shooting'::character varying, 'dribbling'::character varying, 'tacticalAwareness'::character varying, 'defensiveContribution'::character varying, 'physicalStrength'::character varying, 'behavior'::character varying, 'overallPerformance'::character varying])::text[])))
);


ALTER TABLE stf.review_skill_details OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 18959)
-- Name: reviews; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.reviews (
    review_id character varying(50) NOT NULL,
    player_id character varying(50) NOT NULL,
    scout_id character varying(50) NOT NULL,
    match_date date,
    club1_id character varying(50),
    club2_id character varying(50),
    notes text,
    created_at timestamp(0) without time zone DEFAULT now() NOT NULL
);


ALTER TABLE stf.reviews OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 18674)
-- Name: scouts; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.scouts (
    scout_id character varying(50) NOT NULL,
    scout_name character varying(150) NOT NULL,
    role_name character varying(100) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    first_name character varying(100),
    last_name character varying(100),
    email character varying(255),
    phone_number character varying(20),
    address_line1 character varying(255),
    address_line2 character varying(255),
    city character varying(100),
    state character varying(100),
    postal_code character varying(20),
    country character varying(100)
);


ALTER TABLE stf.scouts OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 18739)
-- Name: tasks; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.tasks (
    task_id character varying(50) NOT NULL,
    title character varying(200) NOT NULL,
    description text,
    player_id character varying(50),
    club_id character varying(50),
    assigned_to_scout_id character varying(50) NOT NULL,
    due_date date NOT NULL,
    status character varying(20) NOT NULL,
    source character varying(20) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE stf.tasks OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 18753)
-- Name: templates; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.templates (
    template_id character varying(50) NOT NULL,
    template_name character varying(150) NOT NULL,
    template_type character varying(20) NOT NULL,
    subject character varying(300),
    body text NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE stf.templates OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 18629)
-- Name: users; Type: TABLE; Schema: stf; Owner: postgres
--

CREATE TABLE stf.users (
    id bigint NOT NULL,
    name character varying(150) NOT NULL,
    email character varying(200) NOT NULL,
    password character varying(255) NOT NULL,
    role character varying(20) NOT NULL,
    phone character varying(20),
    status boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT ck_users_role CHECK (((role)::text = ANY ((ARRAY['coach'::character varying, 'scout'::character varying, 'admin'::character varying])::text[])))
);


ALTER TABLE stf.users OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 18628)
-- Name: users_id_seq; Type: SEQUENCE; Schema: stf; Owner: postgres
--

CREATE SEQUENCE stf.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE stf.users_id_seq OWNER TO postgres;

--
-- TOC entry 5218 (class 0 OID 0)
-- Dependencies: 225
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: stf; Owner: postgres
--

ALTER SEQUENCE stf.users_id_seq OWNED BY stf.users.id;


--
-- TOC entry 4967 (class 2604 OID 18172)
-- Name: players id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.players ALTER COLUMN id SET DEFAULT nextval('public.players_id_seq'::regclass);


--
-- TOC entry 4970 (class 2604 OID 18185)
-- Name: users id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- TOC entry 4974 (class 2604 OID 18632)
-- Name: users id; Type: DEFAULT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.users ALTER COLUMN id SET DEFAULT nextval('stf.users_id_seq'::regclass);


--
-- TOC entry 5039 (class 2606 OID 19205)
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- TOC entry 5031 (class 2606 OID 19177)
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- TOC entry 5042 (class 2606 OID 19220)
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- TOC entry 5045 (class 2606 OID 19235)
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- TOC entry 5048 (class 2606 OID 19249)
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- TOC entry 5050 (class 2606 OID 19269)
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- TOC entry 5035 (class 2606 OID 19195)
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- TOC entry 5029 (class 2606 OID 19168)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 4993 (class 2606 OID 18180)
-- Name: players players_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.players
    ADD CONSTRAINT players_pkey PRIMARY KEY (id);


--
-- TOC entry 4995 (class 2606 OID 18200)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 4997 (class 2606 OID 18198)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 5009 (class 2606 OID 18695)
-- Name: club_contacts club_contacts_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.club_contacts
    ADD CONSTRAINT club_contacts_pkey PRIMARY KEY (club_contact_id);


--
-- TOC entry 5003 (class 2606 OID 18671)
-- Name: clubs clubs_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.clubs
    ADD CONSTRAINT clubs_pkey PRIMARY KEY (club_id);


--
-- TOC entry 5011 (class 2606 OID 18712)
-- Name: documents documents_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.documents
    ADD CONSTRAINT documents_pkey PRIMARY KEY (document_id);


--
-- TOC entry 5015 (class 2606 OID 18738)
-- Name: emails emails_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.emails
    ADD CONSTRAINT emails_pkey PRIMARY KEY (email_id);


--
-- TOC entry 5013 (class 2606 OID 18725)
-- Name: notes notes_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.notes
    ADD CONSTRAINT notes_pkey PRIMARY KEY (note_id);


--
-- TOC entry 5025 (class 2606 OID 19015)
-- Name: review_ratings pk_review_ratings; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.review_ratings
    ADD CONSTRAINT pk_review_ratings PRIMARY KEY (review_id);


--
-- TOC entry 5027 (class 2606 OID 19039)
-- Name: review_skill_details pk_review_skill_details; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.review_skill_details
    ADD CONSTRAINT pk_review_skill_details PRIMARY KEY (review_id, skill_key);


--
-- TOC entry 5023 (class 2606 OID 18971)
-- Name: reviews pk_reviews; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.reviews
    ADD CONSTRAINT pk_reviews PRIMARY KEY (review_id);


--
-- TOC entry 5021 (class 2606 OID 18867)
-- Name: players players_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.players
    ADD CONSTRAINT players_pkey PRIMARY KEY (player_id);


--
-- TOC entry 5007 (class 2606 OID 18682)
-- Name: scouts scouts_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.scouts
    ADD CONSTRAINT scouts_pkey PRIMARY KEY (scout_id);


--
-- TOC entry 5017 (class 2606 OID 18752)
-- Name: tasks tasks_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.tasks
    ADD CONSTRAINT tasks_pkey PRIMARY KEY (task_id);


--
-- TOC entry 5019 (class 2606 OID 18764)
-- Name: templates templates_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.templates
    ADD CONSTRAINT templates_pkey PRIMARY KEY (template_id);


--
-- TOC entry 5005 (class 2606 OID 18673)
-- Name: clubs uq_clubs_name; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.clubs
    ADD CONSTRAINT uq_clubs_name UNIQUE (club_name);


--
-- TOC entry 4999 (class 2606 OID 18647)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 5001 (class 2606 OID 18645)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 5033 (class 1259 OID 19280)
-- Name: EmailIndex; Type: INDEX; Schema: auth; Owner: postgres
--

CREATE INDEX "EmailIndex" ON auth."AspNetUsers" USING btree ("NormalizedEmail");


--
-- TOC entry 5037 (class 1259 OID 19275)
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: auth; Owner: postgres
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON auth."AspNetRoleClaims" USING btree ("RoleId");


--
-- TOC entry 5040 (class 1259 OID 19277)
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: auth; Owner: postgres
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON auth."AspNetUserClaims" USING btree ("UserId");


--
-- TOC entry 5043 (class 1259 OID 19278)
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: auth; Owner: postgres
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON auth."AspNetUserLogins" USING btree ("UserId");


--
-- TOC entry 5046 (class 1259 OID 19279)
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: auth; Owner: postgres
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON auth."AspNetUserRoles" USING btree ("RoleId");


--
-- TOC entry 5032 (class 1259 OID 19276)
-- Name: RoleNameIndex; Type: INDEX; Schema: auth; Owner: postgres
--

CREATE UNIQUE INDEX "RoleNameIndex" ON auth."AspNetRoles" USING btree ("NormalizedName");


--
-- TOC entry 5036 (class 1259 OID 19281)
-- Name: UserNameIndex; Type: INDEX; Schema: auth; Owner: postgres
--

CREATE UNIQUE INDEX "UserNameIndex" ON auth."AspNetUsers" USING btree ("NormalizedUserName");


--
-- TOC entry 5058 (class 2606 OID 19206)
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES auth."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- TOC entry 5059 (class 2606 OID 19221)
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES auth."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 5060 (class 2606 OID 19236)
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES auth."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 5061 (class 2606 OID 19250)
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES auth."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- TOC entry 5062 (class 2606 OID 19255)
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES auth."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 5063 (class 2606 OID 19270)
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: auth; Owner: postgres
--

ALTER TABLE ONLY auth."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES auth."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 5051 (class 2606 OID 18696)
-- Name: club_contacts fk_club_contacts_club; Type: FK CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.club_contacts
    ADD CONSTRAINT fk_club_contacts_club FOREIGN KEY (club_id) REFERENCES stf.clubs(club_id) ON DELETE CASCADE;


--
-- TOC entry 5056 (class 2606 OID 19016)
-- Name: review_ratings fk_review_ratings_review; Type: FK CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.review_ratings
    ADD CONSTRAINT fk_review_ratings_review FOREIGN KEY (review_id) REFERENCES stf.reviews(review_id) ON DELETE CASCADE;


--
-- TOC entry 5057 (class 2606 OID 19040)
-- Name: review_skill_details fk_review_skill_details_review; Type: FK CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.review_skill_details
    ADD CONSTRAINT fk_review_skill_details_review FOREIGN KEY (review_id) REFERENCES stf.reviews(review_id) ON DELETE CASCADE;


--
-- TOC entry 5052 (class 2606 OID 18982)
-- Name: reviews fk_reviews_club1; Type: FK CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.reviews
    ADD CONSTRAINT fk_reviews_club1 FOREIGN KEY (club1_id) REFERENCES stf.clubs(club_id);


--
-- TOC entry 5053 (class 2606 OID 18987)
-- Name: reviews fk_reviews_club2; Type: FK CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.reviews
    ADD CONSTRAINT fk_reviews_club2 FOREIGN KEY (club2_id) REFERENCES stf.clubs(club_id);


--
-- TOC entry 5054 (class 2606 OID 18972)
-- Name: reviews fk_reviews_player; Type: FK CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.reviews
    ADD CONSTRAINT fk_reviews_player FOREIGN KEY (player_id) REFERENCES stf.players(player_id) ON DELETE CASCADE;


--
-- TOC entry 5055 (class 2606 OID 18977)
-- Name: reviews fk_reviews_scout; Type: FK CONSTRAINT; Schema: stf; Owner: postgres
--

ALTER TABLE ONLY stf.reviews
    ADD CONSTRAINT fk_reviews_scout FOREIGN KEY (scout_id) REFERENCES stf.scouts(scout_id);


-- Completed on 2026-03-27 14:39:26

--
-- PostgreSQL database dump complete
--

\unrestrict zg2ktAFn97x6FCe9EoHhxHaCFRtJbPcU5QfKaKDS42nQhFZnSIJn58G3LVboqQP

